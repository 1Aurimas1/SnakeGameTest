import { Link } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useState } from "react";
import logo from "../assets/snake_64_bw.png";

const navItems = [
  { name: "Mode selection", path: "/" },
  { name: "Profile", path: "/profile" },
];

export const NavBar = () => {
  const { logout } = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  const mappedLinks = (
    <>
      {navItems.map((item) => (
        <li key={item.name} className="py-2">
          <Link
            to={item.path}
            onClick={() => setIsOpen(false)}
            className="rounded bg-gray-500 px-2 py-1 font-semibold text-white hover:bg-gray-200 hover:text-black active:bg-red-500 active:text-black"
          >
            {item.name}
          </Link>
        </li>
      ))}
      <li className="pt-1">
        <button
          onClick={logout}
          className="rounded bg-gray-500 px-2 py-1 font-bold text-red-500 hover:bg-gray-200 hover:text-black active:bg-red-500 active:text-black"
        >
          Log out
        </button>
      </li>
    </>
  );

  return (
    <nav className="bg-gray-500 p-4">
      <div className="grid grid-cols-2 items-center">
        <div className="w-fit">
          <Link to="/">
            <img className="h-10 w-10" src={logo} alt="My logo" />
          </Link>
        </div>
        <ul className="hidden space-x-4 sm:flex sm:justify-self-end">
          {mappedLinks}
        </ul>

        <div className="flex flex-col sm:hidden">
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="ml-auto text-gray-300 hover:text-white focus:text-white focus:outline-none"
          >
            <svg className="h-6 w-6" strokeWidth={2} stroke="currentColor">
              <path d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
          </button>
        </div>

        {isOpen && (
          <div className="mt-4 sm:hidden">
            <ul>{mappedLinks}</ul>
          </div>
        )}
      </div>
    </nav>
  );
};

export default NavBar;
