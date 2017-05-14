using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;
using System.Collections.Immutable;
using TestApp.State.Actions;

namespace TestApp.State.Reducers
{
  public class ListReducer : Reducer<ListState>
  {
    private ListSearchReducer dataReducer = new ListSearchReducer();
    private ListSelectedItemReducer listReducer = new ListSelectedItemReducer();

    public override ListState Reduce(ListState state = null, object action = null)
    {
      var newState = new ListState(
        search: dataReducer.Reduce(state?.Search, action),
        selectedItem: listReducer.Reduce(state?.SelectedItem, action)
      );

      var hasChanged = state == null
                       || state.Search != newState.Search
                       || state.SelectedItem != newState.SelectedItem;

      return hasChanged ? newState : state;
    }
  }

  public class ListSearchReducer : Reducer<ListSearchState>
  {
    private static readonly ListSearchState initialValue = new ListSearchState(
        searchTerm: "",
        isSearching: false,
        contactIds: ImmutableList<int>.Empty
      );

    public override ListSearchState Reduce(ListSearchState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      switch (action)
      {
        case SearchListAction a:
          return new ListSearchState(
            searchTerm: a.Term,
            isSearching: true,
            contactIds: ImmutableList<int>.Empty //clear previous result
          );
        case SearchListSuccessAction a:
          return new ListSearchState(
            searchTerm: state.SearchTerm,
            isSearching: false,
            contactIds: a.Data
          );
        case SearchListFailAction a:
          return new ListSearchState(
            searchTerm: state.SearchTerm,
            isSearching: false,
            contactIds: ImmutableList<int>.Empty //clear previous result
          );
        default:
          return state;
      }

    }

  }

  public class ListSelectedItemReducer : Reducer<ListSelectedItemState>
  {
    private static readonly ListSelectedItemState initialValue = new ListSelectedItemState(
        selectedId: -1,
        isSelectedSaving: false,
        isSelectedNew: false
    );

    public override ListSelectedItemState Reduce(ListSelectedItemState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      switch (action)
      {
        //select existing or new
        case SelectListItemAction a:
          return new ListSelectedItemState(
            selectedId: a.SelectedId,
            isSelectedSaving: false,
            isSelectedNew: a.IsNew
          );
        //start new, update, delete
        case SaveNewContactAction sa:
        case UpdateContactAction ua:
        case DeleteContactAction da:
          return new ListSelectedItemState(
            selectedId: state.SelectedId,
            isSelectedSaving: true,
            isSelectedNew: state.IsSelectedNew
          );
        //success new, update, delete
        case SaveNewContactSuccessAction sa:
        case UpdateContactSuccessAction ua:
        case DeleteContactSuccessAction da:
          return new ListSelectedItemState(
            selectedId: state.SelectedId,
            isSelectedSaving: false,
            isSelectedNew: state.IsSelectedNew
          );
        //fail new, update, delete
        case SaveNewContactFailAction sa:
        case UpdateContactFailAction ua:
        case DeleteContactFailAction da:
          return new ListSelectedItemState(
            selectedId: state.SelectedId,
            isSelectedSaving: false,
            isSelectedNew: state.IsSelectedNew,
            error: GetErrorForType(action)
          );
        default:
          return state;
      }

    }

    private string GetErrorForType(object action)
    {
      if (action is SaveNewContactFailAction) return "could not add contact";
      if (action is UpdateContactFailAction) return "could not update contact";
      if (action is DeleteContactFailAction) return "could not delete contact";
      else return null;
    }

  }

}