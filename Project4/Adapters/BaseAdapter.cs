using Android.Content;
using Android.Views;

namespace Project4.Adapters
{
    public abstract class BaseAdapter<T> : Android.Widget.BaseAdapter<T>
    {
        protected readonly Context Context;
        protected T[] Items;
        protected readonly int ItemLayoutResource;

        protected BaseAdapter(Context context, int itemLayoutResource)
        {
            Context = context;
            ItemLayoutResource = itemLayoutResource;
            Items = new T[0];
        }

        protected BaseAdapter(Context context, int itemLayoutResource, T[] items)
        {
            Context = context;
            ItemLayoutResource = itemLayoutResource;
            Items = items;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = (LayoutInflater) Context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(ItemLayoutResource, parent, false);
            }

            CreateView(position, convertView, parent);

            return convertView;
        }

        public abstract View CreateView(int position, View convertView, ViewGroup parent);

        public override long GetItemId(int position) => position;

        public override int Count => Items.Length;

        public override T this[int position] => Items[position];

        public void SetItems(T[] items)
        {
            Items = items;
            NotifyDataSetChanged();
        }
    }
}