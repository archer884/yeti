import { Form } from "@remix-run/react";

export default function Login() {
    return (
        <Form action="http://localhost:5000/login" method="post">
            <input type="text" name="username" />
            <input type="text" name="password" />
            <input type="submit" value="Login" />
        </Form>
    );
}
