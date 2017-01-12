using Android.App;
using Android.OS;

namespace GroupSaver
{
    [Activity(Label = "GroupSaver", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //private DataBase.DateBaseLayer dateBase;
        //private VkConnector vkConnector = new VkConnector();
        //private int currentGroupOffset = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

         
            SetContentView(Resource.Layout.Main);

            //dateBase = new DataBase.DateBaseLayer();

            //StartActivity(vkConnector.LoginToVk(this));
 
            //EditText groupNameEditText = FindViewById<EditText>(Resource.Id.editTextGroupName);
            //Button searchGroupButton = FindViewById<Button>(Resource.Id.buttonSearchGroup);

            //List<Group> listGroup;
            //searchGroupButton.Click += delegate
            //{
                //listGroup = vkConnector.SearchGroups(groupNameEditText.Text, currentGroupOffset);
                //var userInfoEditText = FindViewById<EditText>(Resource.Id.editTextAccountInfo);
                //var user = vkConnector.GetLoginedUser();
                //userInfoEditText.Text = user.FirstName + " " + user.LastName;

                //var listGroupsShow = FindViewById<ListView>(Resource.Id.listViewShowGroups);
                //listGroupsShow.Adapter = new ArrayAdapter<Group>(this, Android.Resource.Layout.SimpleListItem1, listGroup);
            //};

        }
    }
}