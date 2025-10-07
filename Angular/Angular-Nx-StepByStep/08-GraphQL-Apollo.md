# 08 — GraphQL & Apollo Client

Use Apollo Angular in an Nx workspace and organize GraphQL code.

## Install Apollo Angular
```powershell
npm i @apollo/client graphql @apollo/angular
```

## Provide Apollo client
```ts
import { APOLLO_OPTIONS } from 'apollo-angular';
import { ApolloClient, InMemoryCache, HttpLink } from '@apollo/client/core';

export function apolloFactory() {
  return new ApolloClient({
    link: new HttpLink({ uri: '/graphql' }),
    cache: new InMemoryCache()
  });
}

bootstrapApplication(AppComponent, {
  providers: [{ provide: APOLLO_OPTIONS, useFactory: apolloFactory }]
});
```

## Query in a service
```ts
import { Apollo, gql } from 'apollo-angular';

@Injectable({ providedIn: 'root' })
export class TodosGql {
  constructor(private apollo: Apollo) {}
  all() {
    return this.apollo.watchQuery<{ todos: { id: string; title: string }[] }>({
      query: gql`query { todos { id title } }`
    }).valueChanges;
  }
}
```

Note: Consider GraphQL codegen for types.
