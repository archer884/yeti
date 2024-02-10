import type { MetaFunction } from "@remix-run/node";
import { LinksFunction, LoaderFunction } from "@remix-run/node";
import { Link, useLoaderData } from "@remix-run/react";
import Header from "../components/Header";

// In case I need to make a call to a protected route, which... probably isn't important on this page.
const jwt = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJpc3MiOiIqIn0.6rinyR0Sb8dqqGmcspbzOuDUSvhi-j2NRej-rZJorn8';

export const loader: LoaderFunction = async () => {
  const response = await fetch("http://localhost:5000/updated");
  const manuscripts = await response.json();
  return manuscripts;
}

export const meta: MetaFunction = () => {
  return [
    { title: "Yeti" },
    { name: "description", content: "Welcome to Yeti!" },
  ];
};

export default function Index() {
  const ms: Manuscript[] = useLoaderData();

  return (
    <main>
      <Header />

      <div style={{ fontFamily: "system-ui, sans-serif", lineHeight: "1.8" }}>
        <h2>Recently updated stories</h2>
        <ul>
          {ms.map((m) => (
            <li key={m.id}>{m.title}</li>
          ))}
        </ul>
      </div>
    </main>
  );
}
