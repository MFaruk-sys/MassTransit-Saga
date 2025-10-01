namespace OrderService.Models;

public record SubmitOrderRequest(string CustomerId, decimal Amount);