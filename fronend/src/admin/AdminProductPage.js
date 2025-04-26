import React, { useEffect, useState } from 'react';
import './admin.css';

const AdminProductPage = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showEditForm, setShowEditForm] = useState(null); // State for edit form
  const [formData, setFormData] = useState({
    id: '',
    name: '',
    price: '',
    description: '',
    stock: '',
    categoryId: '',
    imageFile: null,
    categoryLoadError: '',
  });
  const [error, setError] = useState(null);

  // Default categories as a fallback
  const defaultCategories = [
    { id: 1, name: 'iPhone' },
    { id: 2, name: 'Oppo' },
    { id: 4, name: 'Redmi' },
  ];

  // Fetch products
  const fetchProducts = async () => {
    try {
      const response = await fetch('https://localhost:7083/api/Product');
      if (!response.ok) throw new Error('Failed to fetch products');
      const data = await response.json();
      setProducts(data);
    } catch (error) {
      console.error('Error fetching products:', error);
      setError('Không thể tải sản phẩm.');
    }
  };

  // Fetch categories for dropdown
  const fetchCategories = async () => {
    try {
      const response = await fetch('https://localhost:7083/api/Product/Categories');
      if (!response.ok) throw new Error('Failed to fetch categories');
      const data = await response.json();
      setCategories(data);
    } catch (error) {
      console.error('Error fetching categories:', error);
      setFormData({ ...formData, categoryLoadError: 'Không thể tải danh mục. Sử dụng danh mục mặc định.' });
      setCategories(defaultCategories); // Fallback to default categories
    }
  };

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  // Handle form input changes
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleFileChange = (e) => {
    setFormData({ ...formData, imageFile: e.target.files[0] });
  };

  // Handle form submission for creating a product
  const handleCreateSubmit = async (e) => {
    e.preventDefault();
    const data = new FormData();
    data.append('Name', formData.name);
    data.append('Price', formData.price);
    data.append('Description', formData.description);
    data.append('Stock', formData.stock);
    data.append('CategoryId', formData.categoryId);
    if (formData.imageFile) {
      data.append('ImageFile', formData.imageFile);
    }
    data.append('CategoryLoadError', formData.categoryLoadError);

    try {
      const response = await fetch('https://localhost:7083/api/Product', {
        method: 'POST',
        body: data,
      });
      if (response.ok) {
        const newProduct = await response.json();
        setProducts([...products, newProduct]);
        setShowCreateForm(false);
        setFormData({
          id: '',
          name: '',
          price: '',
          description: '',
          stock: '',
          categoryId: '',
          imageFile: null,
          categoryLoadError: '',
        });
        alert('Thêm sản phẩm thành công!');
      } else {
        const errorData = await response.json();
        alert('Thêm thất bại: ' + (errorData.errors?.join(', ') || errorData.message || 'Lỗi không xác định'));
      }
    } catch (error) {
      console.error('Error creating product:', error);
      alert('Thêm thất bại!');
    }
  };

  // Handle form submission for editing a product
  const handleEditSubmit = async (e) => {
    e.preventDefault();
    const data = new FormData();
    data.append('Id', formData.id);
    data.append('Name', formData.name);
    data.append('Price', formData.price);
    data.append('Description', formData.description);
    data.append('Stock', formData.stock);
    data.append('CategoryId', formData.categoryId);
    if (formData.imageFile) {
      data.append('ImageFile', formData.imageFile);
    }
    data.append('CategoryLoadError', formData.categoryLoadError);
    data.append('Image', ''); // Match Postman request

    try {
      const response = await fetch(`https://localhost:7083/api/Product/${formData.id}`, {
        method: 'PUT',
        body: data,
      });
      if (response.ok) {
        const updatedProduct = await response.json();
        setProducts(products.map((p) => (p.id === updatedProduct.id ? updatedProduct : p)));
        setShowEditForm(null);
        setFormData({
          id: '',
          name: '',
          price: '',
          description: '',
          stock: '',
          categoryId: '',
          imageFile: null,
          categoryLoadError: '',
        });
        alert('Cập nhật sản phẩm thành công!');
      } else {
        const errorData = await response.json();
        alert('Cập nhật thất bại: ' + (errorData.errors?.join(', ') || errorData.message || 'Lỗi không xác định'));
      }
    } catch (error) {
      console.error('Error updating product:', error);
      alert('Cập nhật thất bại!');
    }
  };

  // Handle product deletion
  const handleDelete = async (id) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa sản phẩm này không?')) return;

    try {
      const response = await fetch(`https://localhost:7083/api/Product/${id}`, { method: 'DELETE' });
      if (response.ok) {
        setProducts(products.filter((product) => product.id !== id));
        alert('Xóa thành công!');
      } else {
        throw new Error('Failed to delete');
      }
    } catch (error) {
      console.error('Error deleting product:', error);
      alert('Xóa thất bại!');
    }
  };

  // Handle edit button click
  const handleEditClick = (product) => {
    setFormData({
      id: product.id,
      name: product.name,
      price: product.price,
      description: product.description,
      stock: product.stock,
      categoryId: product.categoryId,
      imageFile: null,
      categoryLoadError: '',
    });
    setShowEditForm(product.id);
  };

  return (
    <div className="admin-product-page">
      <div className="header">
        <h1>Quản lý Sản phẩm</h1>
        <button className="add-button" onClick={() => setShowCreateForm(true)}>
          + Thêm sản phẩm
        </button>
      </div>

      {/* Create Product Form Modal */}
      {showCreateForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Thêm sản phẩm</h2>
            <form onSubmit={handleCreateSubmit}>
              <div className="mb-4">
                <label className="block text-sm font-medium">Tên sản phẩm *</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Giá *</label>
                <input
                  type="number"
                  name="price"
                  value={formData.price}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Mô tả *</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Số lượng *</label>
                <input
                  type="number"
                  name="stock"
                  value={formData.stock}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Danh mục *</label>
                <select
                  name="categoryId"
                  value={formData.categoryId}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                >
                  <option value="">Chọn danh mục</option>
                  {categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.name}
                    </option>
                  ))}
                </select>
                {/* {formData.categoryLoadError && (
                  <p className="text-red-600 text-sm mt-1">{formData.categoryLoadError}</p>
                )} */}
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Hình ảnh</label>
                <input
                  type="file"
                  name="imageFile"
                  onChange={handleFileChange}
                  className="w-full p-2 border rounded"
                  accept="image/*"
                />
              </div>
              <div className="flex gap-4">
                <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
                  Thêm
                </button>
                <button
                  type="button"
                  onClick={() => setShowCreateForm(false)}
                  className="bg-gray-600 text-white px-4 py-2 rounded hover:bg-gray-700"
                >
                  Hủy
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Edit Product Form Modal */}
      {showEditForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Sửa sản phẩm</h2>
            <form onSubmit={handleEditSubmit}>
              <div className="mb-4">
                <label className="block text-sm font-medium">ID</label>
                <input
                  type="number"
                  name="id"
                  value={formData.id}
                  readOnly
                  className="w-full p-2 border rounded bg-gray-100"
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Tên sản phẩm *</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Giá *</label>
                <input
                  type="number"
                  name="price"
                  value={formData.price}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Mô tả *</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Số lượng *</label>
                <input
                  type="number"
                  name="stock"
                  value={formData.stock}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Danh mục *</label>
                <select
                  name="categoryId"
                  value={formData.categoryId}
                  onChange={handleInputChange}
                  className="w-full p-2 border rounded"
                  required
                >
                  <option value="">Chọn danh mục</option>
                  {categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.name}
                    </option>
                  ))}
                </select>
                {formData.categoryLoadError && (
                  <p className="text-red-600 text-sm mt-1">{formData.categoryLoadError}</p>
                )}
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium">Hình ảnh mới</label>
                <input
                  type="file"
                  name="imageFile"
                  onChange={handleFileChange}
                  className="w-full p-2 border rounded"
                  accept="image/*"
                />
              </div>
              <div className="flex gap-4">
                <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
                  Cập nhật
                </button>
                <button
                  type="button"
                  onClick={() => setShowEditForm(null)}
                  className="bg-gray-600 text-white px-4 py-2 rounded hover:bg-gray-700"
                >
                  Hủy
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Product List */}
      <div className="product-grid">
        {products.length > 0 ? (
          products.map((product) => (
            <div key={product.id} className="product-card">
              <img
                src={`https://localhost:7083/images/products/${product.image}`}
                alt={product.name}
                onError={(e) => (e.target.src = '/placeholder-image.jpg')}
              />
              <h2>{product.name}</h2>
              <p className="price">{product.price ? product.price.toLocaleString() + ' VNĐ' : 'Giá chưa có'}</p>
              <p className="description">{product.description}</p>
              <div className="action-buttons">
                <button className="edit-button" onClick={() => handleEditClick(product)}>
                  Sửa
                </button>
                <button onClick={() => handleDelete(product.id)} className="delete-button">
                  Xóa
                </button>
              </div>
            </div>
          ))
        ) : (
          <div className="flex justify-center col-span-full">
            <svg
              className="animate-spin h-8 w-8 text-blue-600"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8v8h8a8 8 0 01-16 0z"
              ></path>
            </svg>
          </div>
        )}
      </div>
      {error && <p className="text-center text-red-600 mt-4">{error}</p>}
    </div>
  );
};

export default AdminProductPage;