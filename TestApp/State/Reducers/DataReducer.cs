using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;
using System.Collections.Immutable;
using TestApp.Models;
using TestApp.State.Actions;

namespace TestApp.State.Reducers
{
  public class DataReducer : Reducer<DataState>
  {
    private static readonly DataState initialValue = new DataState(
        isLoading: false,
        contacts: ImmutableSortedDictionary<int, Contact>.Empty
      );
    public override DataState Reduce(DataState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      switch (action)
      {
        #region LoadContacts
        case LoadContactsAction a:
          return new DataState(
            isLoading: true,
            contacts: state.Contacts
          );
        case LoadContactsSuccessAction a:
          return new DataState(
            isLoading: false,
            contacts: a.Data.ToImmutableSortedDictionary(c => c.Id, c => c)
          );
        case LoadContactsFailAction a:
          return new DataState(
            isLoading: false,
            contacts: ImmutableSortedDictionary<int, Contact>.Empty
          );
        #endregion
        case SaveNewContactSuccessAction a:
          return new DataState(
            isLoading: false,
            contacts: state.Contacts.Add(a.Contact.Id, a.Contact)
          );
        case UpdateContactSuccessAction a:
          var updatedContacts = state.Contacts.Remove(a.Contact.Id);
          updatedContacts = updatedContacts.Add(a.Contact.Id, a.Contact);
          return new DataState(
            isLoading: false,
            contacts: updatedContacts
          );
        case DeleteContactSuccessAction a:
          return new DataState(
          isLoading: false,
          contacts: state.Contacts.Remove(a.ContactId)
        );
        default:
          return state;
      }

    }

  }
}