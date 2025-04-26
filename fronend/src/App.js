import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import logo from './logo.svg';
import './App.css';
import AdminProductPage from './admin/AdminProductPage'; // Fixed import
import Header from './home/Header';
import Footer from './home/Footer';

// Admin component (no Header or Footer)
const Admin = () => {
  return (
    <div className="min-h-screen p-4 bg-gray-100">
      <h2 className="text-2xl font-bold mb-4">Admin Dashboard</h2>
      <p>Welcome to the Admin Panel. Manage your products, users, and settings here.</p>
      {/* Navigation to AdminProductPage */}
      <a href="/admin/products" className="text-blue-600 hover:underline">
        Go to Product Management
      </a>
    </div>
  );
};

function App() {
  return (
    <Router>
      <Routes>
        {/* Home Route with Header and Footer */}
        <Route
          path="/"
          element={
            <div className="flex flex-col min-h-screen">
              <Header />
              <main className="flex-grow p-4">
                <h2 className="text-xl">Welcome to My Website</h2>
                <p>This is a simple page layout with header and footer.</p>
              </main>
              <Footer />
            </div>
          }
        />
        {/* Admin Dashboard Route without Header and Footer */}
        <Route path="/admin" element={<Admin />} />
        {/* Admin Product Management Route */}
        <Route path="/admin/products" element={<AdminProductPage />} />
      </Routes>
    </Router>
  );
}

export default App;