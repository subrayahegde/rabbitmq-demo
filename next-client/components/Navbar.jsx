import Link from 'next/link';
import { useSelector } from 'react-redux';
import styles from '../styles/Navbar.module.css';

const Navbar = () => {
  const cart = useSelector((state) => state.cart);

  const getItemsCount = () => {
    return cart.reduce((accumulator, item) => accumulator + item.quantity, 0);
  };

  return (
    <nav className={styles.navbar}>
      <h6 className={styles.logo}>Next.js Cart</h6>
      <ul className={styles.links}>
        <li className={styles.navlink}>
          <Link href="/">Home</Link>
        </li>       
        <li className={styles.navlink}>
          <Link href="/Cart">
            <a>Cart ({getItemsCount()})</a>
          </Link>
        </li>
        <li className={styles.navlink}>
          <Link href="/About">About</Link>
        </li>        
      </ul>
    </nav>
  );
};

export default Navbar;
