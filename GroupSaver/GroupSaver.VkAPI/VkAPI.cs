using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android.App;
using Android.Content;
using GroupSaver.DateBaseLayer.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Utilities;

namespace GroupSaver.VkAPI
{
    public class VkConnector
    {
        private static string _userId;
        private static string _token;

        public static int NumberOfGroupsLoaded = 50;
        public static int NumberOfPostsLoaded = 30;

        public static string GetUserId()
        {
            return _userId;
        }

        public Intent LoginToVk(Context context)
        {
            var auth = new OAuth2Authenticator(
                clientId: "5810725",
                scope: "groups, friends, wall",
                authorizeUrl: new Uri("https://oauth.vk.com/authorize"),
                redirectUrl: new Uri("https://oauth.vk.com/blank.html")
                );
            auth.Completed += (sender, eventArgs) =>
            {

                if (eventArgs.IsAuthenticated)
                {
                    _userId = eventArgs.Account.Properties["user_id"].ToString();
                    _token = eventArgs.Account.Properties["access_token"].ToString();
                }
                else
                {
                    var builder = new AlertDialog.Builder(context);
                    builder.SetMessage("Not Authenticated");
                    builder.SetPositiveButton("Ok", (o, args) => {} );
                    builder.Create().Show();
                }
            };
            return auth.GetUI(context);
        }

        public List<Group> SearchGroups(string groupName, int offset)
        {
            var request =
                WebRequest.Create("https://api.vk.com/method/groups.search?q=" + groupName + "&type=group&offset=" +
                                  offset + "&count=" + NumberOfGroupsLoaded + "&v=5.62&access_token=" + _token);
            var response = request.GetResponse();

            var defenitionItem = new {id = 0, name = "", screen_name = ""};

            var text = response.GetResponseText();
            var parseJson = JObject.Parse(text);
            var currentObjects = parseJson["response"]["items"].Children().ToList();

           var resultGroups = new List<Group>();
            foreach(var currObject in currentObjects)
            {
                var parsed = JsonConvert.DeserializeAnonymousType(currObject.ToString(), defenitionItem);
                var group = new Group()
                {
                    VkId = parsed.id,
                    Name = parsed.name,
                    ShortUrl = parsed.screen_name
                };
                resultGroups.Add(group);
            }
            return resultGroups;
        }

        public List<Post> GetPosts(int groupVkId, int offset)
        {
            if (_token != string.Empty)
            {
                var request =
                WebRequest.Create("https://api.vk.com/method/wall.get?owner_id=" + (groupVkId*(-1)) + "&offset=" + offset +
                                  "&count=" + NumberOfPostsLoaded + "&filter=owner&extended=0&v=5.62&access_token=" + _token);
                var response = request.GetResponse();

                var defenition = new { id = 0, from_id = 0, owner_id = 0, date = 0, marked_as_ads = 0, post_type = "", text = "" };

                var text = response.GetResponseText();
                var parseJson = JObject.Parse(text);
                var currentObject = parseJson["response"]["items"].Children().ToList();

                var resultPosts = new List<Post>();
                foreach (var currObj in currentObject)
                {
                    var post = JsonConvert.DeserializeAnonymousType(currObj.ToString(), defenition);
                    resultPosts.Add(new Post()
                    {
                        GroupId = -post.owner_id,
                        Text = post.text,
                        VkId = post.id
                    });
                }
                return resultPosts;
            }
            return new List<Post>();
        }
    
        public Person GetLoginedUser()
        {
            if (_token != string.Empty)
            {
                var request =
                WebRequest.Create("https://api.vk.com/method/users.get?user_ids=" + _userId + "&v=5.62&access_token=" +
                                  _token);
                var response = request.GetResponse();

                var defenition = new { id = 0, first_name = "", last_name = "" };

                var text = response.GetResponseText();
                var parseJson = JObject.Parse(text);
                var currentObject = parseJson["response"].Children().ToList();
                foreach (var currObj in currentObject)
                {
                    var person = JsonConvert.DeserializeAnonymousType(currObj.ToString(), defenition);
                    return new Person()
                    {
                        VkId = person.id,
                        FirstName = person.first_name,
                        LastName = person.last_name,
                    };
                }
            }
            return new Person();
        }

    }
}