using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using GroupSaver.DateBaseLayer.Model;

namespace GroupSaver
{
    public class GroupListAdapter:BaseAdapter<Group>
    {
        private readonly Activity _context;
        private readonly List<Group> _list;

        public GroupListAdapter(Activity context, List<Group> list)
        {
            _context = context;
            _list = list;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;           
            var item = this[position];
            if(view == null)
                view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Name;
            return view;
        }

        public override int Count => _list.Count;

        public override Group this[int position] => _list[position];
    }
}