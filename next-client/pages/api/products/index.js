const getURL = 'http://localhost:5087/api/Product';
const postURL = 'http://localhost:5087/api/Product/BuyProducts';
const axios = require('axios');

export async function getProducts() {
  const products = await fetch (getURL);
  if (!products) {
    throw new error ("No Data found.");
   }
   return products.json();
}

export function OrderProducts(cartItems) {
  var newData = "{" + '"' + "cartItems" + '"' + ":";
  newData += JSON.stringify(cartItems);  
  newData += "}";
  
  console.log("newData=" + newData);
  
   axios.post(postURL, newData,
       { headers: {'content-type': 'application/json'} }
    )
    .then(function (response) {
        console.log(response.data);        
    })
    .catch(function (error) {
        console.error(error);
        return ("Not OK");   
    });  
  return ("OK");   
}


export default function handler(req, res) {
  if (req.method !== 'GET') {
    res.setHeader('Allow', ['GET']);
    res.status(405).json({ message: `Method ${req.method} is not allowed` });
  } else {
    const products = getProducts();
    res.status(200).json(products);
  }
}
