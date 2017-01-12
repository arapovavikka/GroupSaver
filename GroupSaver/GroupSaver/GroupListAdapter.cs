using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GroupSaver.DateBaseLayer.Model;

namespace GroupSaver
{
    public class GroupListAdapter:BaseAdapter<Group>
    {
        private Activity _context;
        private List<Group> _list;

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
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
           
            Group item = this[position];
            //view.FindViewById<TextView>(Resource.Id.Title).Text = item.title;
            return view;
        }

        public override int Count { get { return _list.Count; } }

        public override Group this[int position]
        {
            get { return _list[position]; }
        }
    }
}