using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using GroupSaver.DateBaseLayer.Model;

namespace GroupSaver
{
    class PostListAdapter : BaseAdapter<Post>
    {
        private readonly Activity _context;
        private readonly List<Post> _list;

        public PostListAdapter(Activity context, List<Post> posts)
        {
            _context = context;
            _list = posts;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            var item = this[position];
            if (view == null)
                view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Text;
            return view;
        }

        public override int Count => _list.Count;

        public override Post this[int position] => _list[position];
    }
}