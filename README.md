# aspnet-core-sample
A sample project for a c# asp.net core application.

This project uses up:
- [atlas](https://atlasgo.io/) for database set up
- [Auth0](https://auth0.com/) for JWT tokens
- Postgres for the database
- [Dapper](https://github.com/DapperLib/Dapper) for the ORM

## Running
To run locally you'll want to copy `appsettings.json` to `appsettings.Development.json`
and populate the placeholders that are in the template. And then copy
`Properties/launchSettings.template.json` to `Properties/launchSettings.json` and swap
any placeholder values. In theory you should be able to run the server.

## Auth0 Tenant Configuration
The auth0 tenant this template is set up against is configured with Organizations.
The organization id (in your token) leads to the database schema and tenant you target.

## Auth0 JWT Token
This project can use [the auth0 react sample](https://github.com/auth0-samples/auth0-react-samples/tree/master/Sample-01)
to get a valid JWT token for a user. It can be found when hitting the "ping api" in the
external api call (network explorer in developer tools). We can then modify the login
with redirect functionality to include the scopes we want on this [line](https://github.com/auth0-samples/auth0-react-samples/blob/master/Sample-01/src/components/NavBar.js#L78)

```javascript
onClick={() => loginWithRedirect(
  {
    authorizationParams: {
      scope: 'profile email realms:DescribeRealm realms:GetRealms'
    }
  }
```
