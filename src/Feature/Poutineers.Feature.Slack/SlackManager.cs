/********************************************************************************
 * Poutineers 
 * 
 * The Poutineers love mentoring others and love crispy fries with fresh cheese curds covered with hot gravy to melt them just right.
 * 
 * LICENSE: The Poutineeers have developed this for Sitecore Hackathon to share with the world.
 *
 ********************************************************************************/

using System;
using System.Collections.Specialized;
using System.Net;

namespace Poutineers.Feature.Slack
{
    /// <summary>
    /// Assists with getting information from Slack.
    /// </summary>
    public class SlackManager
    {
        /// <summary>
        /// OAuth Token for connecting to Slack
        /// </summary>
        public string OAuthToken { get; set; }

        /// <summary>
        /// Root Server Url for slack (eg. https://poutineers.slack.com)
        /// </summary>
        public string SlackRootUrl { get; set; }

        /// <summary>
        /// Gets the channel history for the given channel name
        /// </summary>
        /// <param name="channelName">Name of channel (this is the internal id)</param>
        /// <returns></returns>
        public string GetChannelHistory(string channelName)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] response = webClient.UploadValues(SlackRootUrl + "/api/channels.history", "POST", new NameValueCollection() {
                                    {"token", OAuthToken},
                                    {"channel",channelName}});
                return System.Text.Encoding.UTF8.GetString(response);
            }
        }

        /// <summary>
        /// Gets the user profile for the given user id
        /// </summary>
        /// <param name="userId">Id of the user (this is the internal id)</param>
        /// <returns></returns>
        public string GetUserProfile(string userId)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] response = webClient.UploadValues(SlackRootUrl + "/api/users.profile.get", "POST", new NameValueCollection() {
                                        {"token",OAuthToken},
                                        {"user",userId}});
                return System.Text.Encoding.UTF8.GetString(response);
            }
        }
    }
}
