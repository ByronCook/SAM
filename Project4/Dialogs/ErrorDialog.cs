using System;
using Android.App;
using Android.Content;

namespace Project4.Dialogs
{
    public class ErrorDialog
    {
        private readonly AlertDialog.Builder _builder;

        public int TitleRes { get; set; }
        public int MessageRes { get; set; }
        public int ButtonRes { get; set; }

        public ErrorDialog(Context context)
        {
            _builder = new AlertDialog.Builder(context);
        }

        public ErrorDialog(Context context, int titleRes, int messageRes)
        {
            _builder = new AlertDialog.Builder(context);
            TitleRes = titleRes;
            MessageRes = messageRes;
        }

        public void Display()
        {
            _builder.SetTitle(TitleRes == 0 ? Resource.String.dlg_default_error_title : TitleRes);
            _builder.SetMessage(MessageRes == 0 ? Resource.String.dlg_default_error : MessageRes);
            _builder.SetNegativeButton(ButtonRes == 0 ? Resource.String.btn_ok : ButtonRes, ClickHandler);
            _builder.Create();
            _builder.Show();
        }

        private void ClickHandler(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            (sender as AlertDialog)?.Dismiss();
        }
    }
}