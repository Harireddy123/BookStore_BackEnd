using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Service
{
    public class WishlistRL : IWishlistRL
    {
        private readonly BookContext _context;

        public WishlistRL(BookContext context)
        {
            _context = context;
        }

        public WishList AddToWishlist(int userId, int bookId)
        {
            var wishList = new WishList
            {
                UserId = userId,
                BookId = bookId
            };
            _context.Wishlists.Add(wishList);
            _context.SaveChanges();

            // After saving, fetch Wishlist again INCLUDING Book details
            var result = _context.Wishlists
                .Include(w => w.Book)   // 👈 Include Book
                .FirstOrDefault(w => w.WishlistId == wishList.WishlistId);

            return result;
        }

        public bool RemoveFromWishlist(int wishlistId, int userId)
        {
            var item = _context.Wishlists.FirstOrDefault(w => w.WishlistId == wishlistId && w.UserId == userId);
            if (item == null) return false;

            _context.Wishlists.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public List<WishList> GetWishlistByUserId(int userId)
        {
            return _context.Wishlists
          .Where(w => w.UserId == userId)
          .Include(w => w.Book)   // 👈 Include Book while fetching
          .ToList();
        }
    }
}
