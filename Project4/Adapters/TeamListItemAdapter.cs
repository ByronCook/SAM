using Android.Content;
using Android.Views;
using Android.Widget;
using Project4.Models;

namespace Project4.Adapters
{
    public class TeamListItemAdapter : BaseAdapter<TeamListItem>
    {
        private TextView _title;
        private TextView _subText;

        public TeamListItemAdapter(Context context, int itemLayoutResource) : base(context, itemLayoutResource)
        {

        }

        public override View CreateView(int position, View convertView, ViewGroup parent)
        {
            var teamListItem = Items[position];
            _title = convertView.FindViewById<TextView>(Resource.Id.item_title);
            _subText = convertView.FindViewById<TextView>(Resource.Id.item_subtext);

            _title.Text = teamListItem.Team.Name;
            _subText.Text = teamListItem.Club.Name;

            return convertView;
        }
    }
}