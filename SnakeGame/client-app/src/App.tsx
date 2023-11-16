import { BrowserRouter, Route, Routes } from "react-router-dom";

import { AuthProvider } from "./hooks/useAuth";
import RequireAuth from "../src/components/RequireAuth";

import Login from "./pages/Login";
import Register from "./pages/Register";
import GameModeSelection from "./pages/GameModeSelection";
import GameRoom from "./pages/GameRoom";
import Profile from "./pages/Profile";
import NotFound from "./pages/NotFound";

import Layout from "./components/Layout";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route>
              <Route element={<Layout />}>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route element={<RequireAuth />}>
                <Route path="/" element={<GameModeSelection />} />
                <Route path="/play" element={<GameRoom />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="*" element={<NotFound />} />
            </Route>
              </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
