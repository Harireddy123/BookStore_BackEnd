using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class WishlistBL : IWishlistBL
    {
        private readonly IWishlistRL _wishlistRL;

        public WishlistBL(IWishlistRL wishlistRL)
        {
            _wishlistRL = wishlistRL;
        }

        public WishList AddToWishlist(int userId, int bookId) =>
            _wishlistRL.AddToWishlist(userId, bookId);

        public bool RemoveFromWishlist(int wishlistId, int userId) =>
            _wishlistRL.RemoveFromWishlist(wishlistId, userId);

        public List<WishList> GetWishlistByUserId(int userId) =>
            _wishlistRL.GetWishlistByUserId(userId);
    }
}
