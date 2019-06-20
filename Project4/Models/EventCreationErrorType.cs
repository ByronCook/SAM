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

namespace Project4.Models
{
    enum EventCreationErrorType
    {
        NoResult,
        NoError,
        NoName,
        NoStartDate,
        NoEndDate,
        NoType,
        EventAlreadyExists,
        PrimaryDateSmallerThanSecondary,
        DateAlreadyPassed
    }
}