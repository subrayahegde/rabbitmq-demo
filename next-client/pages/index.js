import ProductCard from '../components/ProductCard';
import styles from '../styles/ShopPage.module.css';
import { getProducts } from './api/products/index';


const HomePage = ({ products }) => {
  return (
    <div className={styles.container}>
      <h1>Products List</h1>
      <br/><br/>
      <hr/>
      <br/><br/>
      
      <div className={styles.cards}>
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
};

export default HomePage;

export async function getStaticProps() {
  const products = await getProducts();
  return { props: { products } };
}
