using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using GroupSaver.DateBaseLayer.Model;
using GroupSaver.VkAPI;
using Newtonsoft.Json;

namespace GroupSaver
{
    [Activity(Label = "PostsActivity")]
    public class PostsActivity : Activity
    {
        private VkConnector _vkConnector;
        private Group _selectedGroup;
        private List<Post> _posts;
        private Person _user;
        private DataBase.DataBaseLayer _db;
        private int _currentPostOffset;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PostsLayout);

            if (ParseBundle())
            {
                CreateDataBase();
                _user = _db.GetPersonByVkId(int.Parse(VkConnector.GetUserId()));

                // set group name
                var groupNameTextView = FindViewById<TextView>(Resource.Id.textViewGroupName);
                groupNameTextView.Text = _selectedGroup.Name;

                _vkConnector = new VkConnector();
                _currentPostOffset = 0;
                _posts = new List<Post>();
                SetPostsInListView();

                var subscribeButton = FindViewById<Button>(Resource.Id.buttonSubscribe);

                // get information about group subscribe
                if (_db.HaveGroupVkId(_selectedGroup.VkId))
                {
                    _selectedGroup = _db.GetGroupByVkId(_selectedGroup.VkId);
                    if (_db.HaveSubscribe(_selectedGroup.Id, _user.Id))
                    {
                        subscribeButton.Text = "-";
                    }
                }

                subscribeButton.Click += delegate
                {
                    AddOrRemoveSubscribe();
                };

                var loadMorePostsButton = FindViewById<Button>(Resource.Id.buttonLoadMorePosts);
                loadMorePostsButton.Click += delegate
                {
                    SetPostsInListView();
                };
            }
        }

        private void CreateDataBase()
        {
            _db = new DataBase.DataBaseLayer();
            _db.CreateGroupTable();
            _db.CreatePersonTable();
            _db.CreatePostTable();
            _db.CreateGroupSubscribeTable();
        }

        private bool ParseBundle()
        {
            var defenitionGroup = new { VkId = 0, Name = "", ShortUrl = "" };
            var selectedGroup = Intent.GetStringExtra("group");
            var groupObject = JsonConvert.DeserializeAnonymousType(selectedGroup, defenitionGroup);

            _selectedGroup = new Group()
            {
                VkId = groupObject.VkId,
                Name = groupObject.Name,
                ShortUrl = groupObject.ShortUrl
            };

            return _selectedGroup.VkId != 0 && _selectedGroup.Name != string.Empty;
        }

        private void SetPostsInListView()
        {
            _posts.AddRange(_vkConnector.GetPosts(_selectedGroup.VkId, _currentPostOffset));
            _currentPostOffset = _currentPostOffset + VkConnector.NumberOfPostsLoaded;
  
            var postsListView = FindViewById<ListView>(Resource.Id.listViewPosts);
            if (_posts.Any())
            {
                postsListView.Adapter = new PostListAdapter(this, _posts);
            }
            else
            {
                Toast.MakeText(this, "No posts", ToastLength.Short).Show();
            }
        }

        private void AddOrRemoveSubscribe()
        {
            var subscribeButton = FindViewById<Button>(Resource.Id.buttonSubscribe);
            if (subscribeButton.Text == "+")
            {
                //add in database new subscribe
                _db.AddGroup(new Group()
                {
                    Name = _selectedGroup.Name,
                    ShortUrl = _selectedGroup.ShortUrl,
                    VkId = _selectedGroup.VkId
                });

                _selectedGroup = _db.GetGroupByVkId(_selectedGroup.VkId);
                if (_db.AddGroupSubscribe(_user.Id, _selectedGroup.Id))
                    subscribeButton.Text = "-";
            }
            else
            {
                //remove subscribe
                if (_db.DeleteSubscribe(_selectedGroup.Id, _user.Id))
                {
                    _db.DeleteGroup(_selectedGroup.Id);
                    subscribeButton.Text = "+";
                }
            }
        }

        public override void OnBackPressed()
        {
            this.Finish();
        }

    }
}