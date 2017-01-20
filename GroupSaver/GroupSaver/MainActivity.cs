using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using GroupSaver.DateBaseLayer.Model;
using GroupSaver.VkAPI;
using Newtonsoft.Json;


namespace GroupSaver
{
    [Activity(Label = "GroupSaver",MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private readonly VkConnector _vkConnector = new VkConnector();
        private DataBase.DataBaseLayer _db;
        private int _currentGroupOffset;
        private Person _user = new Person();
        private List<Group> _groups = new List<Group>();
        private bool _listViewSearch;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            StartActivity(_vkConnector.LoginToVk(this));

            ConnectToDb();

            var searchGroupButton = FindViewById<Button>(Resource.Id.buttonSearchGroup);
            var favouriteGroupsButton = FindViewById<Button>(Resource.Id.buttonFavourite);
            var loadMoreGroupsButton = FindViewById<Button>(Resource.Id.buttonLoadMoreGroups);
            _listViewSearch = true;

            searchGroupButton.Click += delegate
            {
                _listViewSearch = true;
                if (_user.Id == 0)
                {
                    GetUserInfo();
                }
                SearchGroups(true);
            };

            favouriteGroupsButton.Click += delegate
            {
                _listViewSearch = false;
                if (_user.Id == 0)
                {
                    GetUserInfo();
                }
                GetFavouriteGroups();
            };

            loadMoreGroupsButton.Click += delegate
            {
                if (_user.Id == 0)
                {
                    GetUserInfo();
                }

                if (_listViewSearch)
                {
                    SearchGroups(false);
                }
                else
                {
                    Toast.MakeText(this, "All favourite groups", ToastLength.Short).Show();
                }
            };
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var listGroups = FindViewById<ListView>(Android.Resource.Id.List);
            if (listGroups.Count != 0)
            {
                var intent = new Intent(this, typeof(PostsActivity));
                var groupToString = JsonConvert.SerializeObject(_groups.ElementAt(position));

                intent.PutExtra("group", groupToString);
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Nothing found", ToastLength.Short).Show();
            }
        }

        private void ConnectToDb()
        {
            _db = new DataBase.DataBaseLayer();
            _db.CreateGroupTable();
            _db.CreatePersonTable();
            _db.CreateGroupSubscribeTable();
        }

        private void GetUserInfo()
        {
            var userInfoTextView = FindViewById<TextView>(Resource.Id.textViewUserInfo);
            _user = _vkConnector.GetLoginedUser();

            _db.AddPerson(new Person()
            {
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                VkId = _user.VkId
            });

            _user = _db.GetPersonByVkId(_user.VkId);
            userInfoTextView.Text = _user.FirstName + " " + _user.LastName;
        }

        private void SearchGroups(bool newSearch)
        {
            var groupNameEditText = FindViewById<EditText>(Resource.Id.editTextGroupName);
            var listGroups = FindViewById<ListView>(Android.Resource.Id.List);

            if (newSearch)
            {
                _groups.Clear();
                _currentGroupOffset = 0;
            }

            if (groupNameEditText.Text != string.Empty)
            {
                _groups.AddRange(_vkConnector.SearchGroups(groupNameEditText.Text, _currentGroupOffset));
                _currentGroupOffset = _currentGroupOffset + VkConnector.NumberOfGroupsLoaded;

                if (_groups.Any())
                {
                    listGroups.Adapter = new GroupListAdapter(this, _groups);
                }
                else
                {
                    Toast.MakeText(this, "Nothing found", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Empty group name", ToastLength.Short).Show();
            }
        }

        private void GetFavouriteGroups()
        {
            var listGroups = FindViewById<ListView>(Android.Resource.Id.List);
            if (_user.Id != 0)
            {
                _groups.Clear();
                _groups = _db.GetAllUserGroups(_user.Id);
                if (_groups.Any())
                {
                    listGroups.Adapter = new GroupListAdapter(this, _groups);
                }
                else
                {
                    Toast.MakeText(this, "Nothing found", ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Load user info first", ToastLength.Short).Show();
            }
        }
    }
}