/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;

namespace Poutineers.Feature.XConnect
{
    public class XConnectManager
    {
        /// <summary>
        /// The xConnectClient to interact with
        /// </summary>
        public XConnectClient Client { get; set; }

        /// <summary>
        /// Gets a contact from the xDb using xConnect
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Contact GetContact(string contactId)
        {
            var reference = new ContactReference(Guid.Parse(contactId));

            Sitecore.XConnect.Contact xConnectContact = Client.Get(reference, new ContactExpandOptions() { });

            // TO DO: If there are any other fields to add

            return xConnectContact;
        }

        public Contact GetContactByIndentifier(string sourceType, string userId, bool includeListManager)
        {
            var reference = new IdentifiedContactReference(sourceType, userId);
            var contact = Client.Get<Contact>(reference, new ContactExpandOptions(new string[1] { "ListSubscriptions" }));

            return contact;
        }

        /// <summary>
        /// Inserts Interaction on existing contact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="slackId"></param>
        /// <param name="slackDateTime"></param>
        /// <param name="messageText"></param>
        /// <returns></returns>
        public bool AddContactToList(Contact knownContact, string listId)
        {
            // if contact does not have list subscriptions then we need to add it
            if (knownContact.ListSubscriptions() == null)
            {
                var listSubscriptions = new ListSubscriptions();
                listSubscriptions.Subscriptions.Add(new ContactListSubscription(DateTime.Now, true, new Guid(listId)));
                Client.SetFacet<ListSubscriptions>(knownContact, ListSubscriptions.DefaultFacetKey, listSubscriptions);
            }
            else
            {
                knownContact.ListSubscriptions().Subscriptions.Add(new ContactListSubscription(DateTime.Now, true, new Guid(listId)));
            }

            Client.Submit();

            return true;
        }

        /// <summary>
        /// Inserts a contact into xDb using xConnect
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public bool InsertContactAndAddContactToList(string source, string contactId, string alias, string firstName, string lastName, string title, string emailAddress, string listId)
        {
            // Identifier for a 'known' contact
            var identifier = new ContactIdentifier[]
            {
                new ContactIdentifier(source, contactId, ContactIdentifierType.Known),
                new ContactIdentifier(source + "_ALIAS", alias, ContactIdentifierType.Known)
            };

            // Create a new contact with the identifier
            var knownContact = new Sitecore.XConnect.Contact(identifier);

            var personalInfoFacet = new PersonalInformation()
            {
                Title = title,
                FirstName = firstName,
                LastName = lastName
            };


            Client.SetPersonal(knownContact, personalInfoFacet);

            var emailAddressList = new EmailAddressList(new EmailAddress(emailAddress, false), "SlackEmailAddress");
            Client.SetEmails(knownContact, emailAddressList);

            // if contact does not have list subscriptions then we need to add it
            if (knownContact.ListSubscriptions() == null)
            {
                var listSubscriptions = new ListSubscriptions();
                listSubscriptions.Subscriptions.Add(new ContactListSubscription(DateTime.Now, true, new Guid(listId)));
                Client.SetFacet<ListSubscriptions>(knownContact, ListSubscriptions.DefaultFacetKey, listSubscriptions);
            }
            else
            {
                knownContact.ListSubscriptions().Subscriptions.Add(new ContactListSubscription(DateTime.Now, true, new Guid(listId)));
            }

            Client.AddContact(knownContact);

            Client.Submit();

            // Get the last batch that was executed
            var operations = Client.LastBatch;

            return true;
        }
    }
}
