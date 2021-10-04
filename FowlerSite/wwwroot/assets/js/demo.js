function getItemSubtotal() {
    var price = Number(document.getElementById("itemPrice").innerHTML);
    var quantity = Number(document.getElementById("itemQuantity").querySelector('input').value);
    var itemTotal = price * quantity;

    return itemTotal;
}