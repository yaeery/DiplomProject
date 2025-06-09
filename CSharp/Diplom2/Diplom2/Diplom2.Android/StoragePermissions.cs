using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Core.App;
using Xamarin.Forms;
using Android;

[assembly: Dependency(typeof(Diplom2.Droid.StoragePermissions))]

namespace Diplom2.Droid
{
    internal class StoragePermissions : IPermissions
    {
        string[] StoragePermissionsList => new string[]
        {
            Manifest.Permission.ManageExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage
        };

        Activity activity;
        public void RequestPermissions()
        {
            if (activity == null) activity = (Activity)Forms.Context;
            activity.RequestPermissions(StoragePermissionsList, 0);
            ActivityCompat.RequestPermissions(activity, StoragePermissionsList, 1);
            for (int i = 0; i < StoragePermissionsList.Length; i++)
            {
                string Permission = StoragePermissionsList[i];
                ActivityCompat.RequestPermissions(activity, new string[] { Permission }, 1);
            }
        }
    }
}