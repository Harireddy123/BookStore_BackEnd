using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;

namespace BussinessLayer.Interface
{
    public interface IWishlistBL
    {
        WishList AddToWishlist(int userId, int bookId);
        bool RemoveFromWishlist(int wishlistId, int userId);
        List<WishList> GetWishlistByUserId(int userId);
    }
}
