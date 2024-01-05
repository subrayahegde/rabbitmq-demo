import styles from '../styles/CartPage.module.css';
import { useSelector, useDispatch } from 'react-redux';
import {
  incrementQuantity,
  decrementQuantity,
  removeFromCart,
  resetCart
} from '../redux/cart.slice';
import { OrderProducts } from './api/products/index';

function CartItem(id, qty) {
    this.id = id;
    this.qty = qty;
}

function CartItems(cartItem) {
    this.cartItem = cartItem;
}

const handleClick = (event, dispatch, cart, param) => {  
    
    let cartItems = [];
    
    for (var i=0; i<cart.length; i++) {     
      let cartItem = new CartItem(cart[i].id, cart[i].quantity);
      cartItems.push(cartItem);
    }
   
    const msg = OrderProducts(cartItems);
   
    if (msg == "OK") {
       alert('Product(s) are ordered. Total Cost: $' + param);   
       dispatch(resetCart());
    }
  
  };
  
const CartPage = () => {
  const cart = useSelector((state) => state.cart);
  const dispatch = useDispatch();

  const getTotalPrice = () => {
    return cart.reduce(
      (accumulator, item) => accumulator + item.quantity * item.price,
      0
    );
  };

  return (
    <div className={styles.container}>
      {cart.length === 0 ? (
        <h1>Your Cart is Empty!</h1>
      ) : (
        <>
          <div className={styles.header}>       
            <div>Name</div>
            <div>Description</div>            
            <div>Price</div>
            <div>Quantity</div>
            <div>Actions</div>
            <div>Total Price</div>
          </div>
          {cart.map((item) => (
            <div className={styles.body}> 
              <p>{item.name}</p>
              <p>{item.description}</p>
              <p>$ {item.price}</p>
              <p>{item.quantity}</p>
              <div className={styles.buttons}>               
                <button onClick={() => dispatch(decrementQuantity(item.id))}>
                  -
                </button>
                 <button onClick={() => dispatch(incrementQuantity(item.id))}>
                  +
                </button>
                <button onClick={() => dispatch(removeFromCart(item.id))}>
                  x
                </button>
              </div>
              <p>$ {(item.quantity * item.price).toFixed(2)}</p>
            </div>
          ))}
          <h2>Grand Total: $ {getTotalPrice().toFixed(2)}</h2>
          <br/>            
           <button onClick={event => handleClick(event, dispatch, cart, getTotalPrice().toFixed(2))}>Check Out</button>  
           <button onClick={() => dispatch(resetCart())}>Empty Cart</button>
        </>
      )}     
      
    </div>
  );
};

export default CartPage;

