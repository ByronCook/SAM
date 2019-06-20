using Android.Content;
using Android.Views;
using Android.Widget;
using Project4.Fragments;
using Project4.Models;

namespace Project4.Adapters
{
    public class AttendantListItemAdapter : BaseAdapter<AttendantListItem>
    {
        public AttendantListItemAdapter(Context context, int itemLayoutResource) : base(context, itemLayoutResource)
        {

        }

        public override View CreateView(int position, View convertView, ViewGroup parent)
        {
            var item = Items[position];

            var title = convertView.FindViewById<TextView>(Resource.Id.item_title);
            var subText = convertView.FindViewById<TextView>(Resource.Id.item_subtext);
            var icon = convertView.FindViewById<ImageView>(Resource.Id.item_icon);

            var name = item.User.Firstname + " " + item.User.Lastname;
            
            title.Text = name == " " ? item.User.Username : name;

            subText.Text = item.Reason;

            switch (item.AttendState)
            {
                case EventAttendingFragment.AttendingState.Yes:
                    icon.SetImageResource(Resource.Drawable.checkmark);
                    break;
                case EventAttendingFragment.AttendingState.Maybe:
                    icon.SetImageResource(Resource.Drawable.questionmark);
                    break;
                case EventAttendingFragment.AttendingState.No:
                    icon.SetImageResource(Resource.Drawable.cancel);
                    break;
            }


            return convertView;
        }
    }
}