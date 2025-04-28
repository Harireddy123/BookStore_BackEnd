using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IWishlistRL
    {
        WishList AddToWishlist(int userId, int bookId);
        bool RemoveFromWishlist(int wishlistId, int userId);
        List<WishList> GetWishlistByUserId(int userId);
    }
}
