﻿using MassTransit;
using MediatR;
using Messaging.Events;
using Security.Services;
using ShoppingCart.API.Dtos;
using ShoppingCart.API.Services;

namespace ShoppingCart.API.ShoppingCart
{
    public record CheckoutCartRequest(CartCheckoutDto CartCheckoutDto) : IRequest<IResult>;
    public record CheckoutBasketResult(CartCheckoutDto CartCheckoutDto); 

    public sealed class CheckoutCartHandler(IShoppingCartService service,
        IUserContext userContext,
        IPublishEndpoint publishEndpoint
        ) : IRequestHandler<CheckoutCartRequest, IResult>
    {
        public async Task<IResult> Handle(CheckoutCartRequest checkoutCartRequest, CancellationToken cancellationToken)
        {
            var cart = await service.GetOrCreateCartAsync(cancellationToken);
            var userId = userContext.GetUserId();
            var eventMessage = new CartCheckoutEvent()
            {
                AddressLine = checkoutCartRequest.CartCheckoutDto.AddressLine,
                CardName = checkoutCartRequest.CartCheckoutDto.CardName,
                CardNumber = checkoutCartRequest.CartCheckoutDto.CardNumber,
                Country = checkoutCartRequest.CartCheckoutDto.Country,
                CVV = checkoutCartRequest.CartCheckoutDto.CVV,
                EmailAddress = checkoutCartRequest.CartCheckoutDto.EmailAddress,
                PhoneNumber = checkoutCartRequest.CartCheckoutDto.PhoneNumber,
                Expiration = checkoutCartRequest.CartCheckoutDto.Expiration,
                State = checkoutCartRequest.CartCheckoutDto.State,
                ZipCode = checkoutCartRequest.CartCheckoutDto.ZipCode,
                UserId = userId ?? null
            };

            for (int i = 0; i < cart.Selections.Count; i++)
            {
                eventMessage.ProductSelections.Add(new ProductSelectionDto()
                {
                    ProductId = cart.Selections[i].ProductId,
                    ProductName = cart.Selections[i].ProductName,
                    Price = cart.Selections[i].Price,
                    Quantity = cart.Selections[i].Quantity
                });
            }

            await publishEndpoint.Publish(eventMessage, cancellationToken);
            await service.DeleteCartAsync(cart.Id, cancellationToken);
            return Results.Ok();
        }
    }
}
