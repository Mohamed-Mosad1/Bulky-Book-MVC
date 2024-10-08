﻿// Ignore Spelling: BLL

using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.ShoppingCarts;
using BulkyBook.Model.Cart;
using System.Transactions;

namespace BulkyBook.BLL.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddOrUpdateToCartAsync(string userId, int productId, int quantity)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var shoppingCart = await GetCartAsync(userId, true);
                if (shoppingCart is null)
                {
                    shoppingCart = new ShoppingCart()
                    {
                        AppUserId = userId
                    };

                    _unitOfWork.Repository<ShoppingCart>().Add(shoppingCart);
                    await _unitOfWork.CompleteAsync();
                }

                var cartItem = shoppingCart.CartItems.FirstOrDefault(x => x.ProductId == productId);

                if (cartItem is not null)
                {
                    cartItem.Quantity += quantity;
                    _unitOfWork.Repository<ShoppingCartItem>().Update(cartItem);
                }
                else
                {
                    var newCartItem = new ShoppingCartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        ShoppingCartId = shoppingCart.Id
                    };

                    _unitOfWork.Repository<ShoppingCartItem>().Add(newCartItem);
                }

                await _unitOfWork.CompleteAsync();
                transaction.Complete();
            }
        }

        public async Task<ShoppingCart?> GetCartAsync(string userId, bool includeCartItem = false, bool includeProduct = false, bool includeImages = false)
        {
            var spec = new ShoppingCartWithCartItemSpec(userId, includeCartItem, includeProduct, includeImages);
            return await _unitOfWork.Repository<ShoppingCart>().GetWithSpecAsync(spec);
        }

        public async Task<ShoppingCartItem?> GetCartItemByIdAsync(string cartItemId)
        {
            return await _unitOfWork.Repository<ShoppingCartItem>().GetByIdAsync(cartItemId);
        }

        public async Task RemoveCartItemAsync(string cartItemId)
        {
            var shoppingCartItem = await GetCartItemByIdAsync(cartItemId);
            if (shoppingCartItem is not null)
            {
                _unitOfWork.Repository<ShoppingCartItem>().Delete(shoppingCartItem);
                await _unitOfWork.CompleteAsync();

                if (shoppingCartItem?.Quantity <= 1)
                {
                    await RemoveCartAsync(shoppingCartItem.ShoppingCartId);
                }
            }

        }

        public async Task RemoveCartAsync(string cartId)
        {
            var shoppingCart = await _unitOfWork.Repository<ShoppingCart>().GetByIdAsync(cartId);
            if (shoppingCart is not null)
            {
                _unitOfWork.Repository<ShoppingCart>().Delete(shoppingCart);
                await _unitOfWork.CompleteAsync();
            }
        }

        public decimal GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.Quantity switch
            {
                <= 50 => shoppingCartItem.Product.Price,
                <= 100 => shoppingCartItem.Product.Price50,
                _ => shoppingCartItem.Product.Price100
            };
        }

        public async Task IncrementCartItemAsync(string cartItemId)
        {
            var cartItem = await _unitOfWork.Repository<ShoppingCartItem>().GetByIdAsync(cartItemId);
            if (cartItem is not null)
            {
                cartItem.Quantity += 1;
                _unitOfWork.Repository<ShoppingCartItem>().Update(cartItem);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DecrementCartItemAsync(string cartItemId)
        {
            var cartItem = await GetCartItemByIdAsync(cartItemId);

            if (cartItem is not null)
            {
                if (cartItem.Quantity <= 1)
                {
                    var cart = await _unitOfWork.Repository<ShoppingCart>().GetByIdAsync(cartItem.ShoppingCartId);

                    if (cart is not null)
                        _unitOfWork.Repository<ShoppingCart>().Delete(cart);
                }
                else
                {
                    cartItem.Quantity -= 1;
                    _unitOfWork.Repository<ShoppingCartItem>().Update(cartItem);
                }

                await _unitOfWork.CompleteAsync();
            }
        }


        public async Task<int> GetCountAsync(string userId)
        {
            var shoppingCart = await GetCartAsync(userId, true);
            return shoppingCart?.CartItems.Sum(x => x.Quantity) ?? 0;
        }

    }
}
