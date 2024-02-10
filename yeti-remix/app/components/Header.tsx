import React from "react";
import { Link } from "@remix-run/react";

export default function Header() {
  return (
    <div>
      <h1>Yeti</h1>
      <ul>
        <li><Link to="/login">Login</Link></li>
      </ul>
    </div>
  );
}
