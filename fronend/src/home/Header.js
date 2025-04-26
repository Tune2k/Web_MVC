import React, { useState, useEffect } from 'react';
import './header.css'
const Header = () => {
  // State để lưu danh sách sản phẩm
  const [products, setProducts] = useState([]);

  // Hàm gọi API
  const fetchProducts = async () => {
    try {
      const response = await fetch('https://localhost:7083/api/Product');
      const data = await response.json();
      setProducts(data); // Lưu danh sách sản phẩm vào state
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  // Gọi API khi component mount
  useEffect(() => {
    fetchProducts();
  }, []);

  return (
    <header className="bg-blue-600 text-white p-6 shadow-md">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">My Website</h1>
        <nav>
          <ul className="flex gap-6">
            <li><a href="/" className="hover:underline">Home</a></li>
            <li><a href="/about" className="hover:underline">About</a></li>
            <li><a href="/contact" className="hover:underline">Contact</a></li>
          </ul>
        </nav>
      </div>

      {/* Hiển thị danh sách sản phẩm nếu đã tải */}
      <div className="product-list grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 mt-8">
        {products.length > 0 ? (
          products.map((product) => (
            <div key={product.id} className="product-info border border-gray-300 bg-white p-4 rounded-lg hover:shadow-lg transition duration-300 text-black">
              <img
                src={`https://localhost:7083/images/products/${product.image}`}
                alt={product.name}
                className="w-full h-48 object-cover rounded-md"
              />
              <h2 className="text-xl font-semibold mt-4">{product.name}</h2>
              <p className="text-lg text-green-600 mt-2">
                {product.price ? product.price.toLocaleString() + ' VNĐ' : 'Giá chưa có'}
              </p>
              <p className="text-sm text-gray-600 mt-2">{product.description}</p>
            </div>
          ))
        ) : (
          <p className="text-center col-span-full">Loading...</p>
        )}
      </div>
    </header>
  );
};

export default Header;
