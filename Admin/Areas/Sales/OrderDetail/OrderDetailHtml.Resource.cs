using System;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Templates
{
    public static partial class OrderDetailHtml
    {
        #region Helpers

        private static FileResource GetResourceByName(String fileName)
        {
            var resource = new FileResource($@"Areas\Sales\OrderDetail\Templates\{fileName}");
            return resource;
        }

        #endregion
    }
}