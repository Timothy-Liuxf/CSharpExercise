using System.Diagnostics.CodeAnalysis;

namespace GoScript.Frontend.Runtime
{
    internal class ScopeStack
    {
        private readonly LinkedList<Scope> scopes;

        public ScopeStack()
        {
            scopes = new LinkedList<Scope>();
            scopes.AddLast(new Scope());    // Init global scope
        }

        public RTTI? LookUp(string name)
        {
            foreach (var scope in this.scopes.Reverse())
            {
                if (scope.Symbols.TryGetValue(name, out var rtti))
                {
                    return rtti;
                }
            }
            return null;
        }

        public bool TryLookUp(string name, [MaybeNullWhen(false), NotNullWhen(true)] out RTTI? rtti)
        {
            rtti = LookUp(name);
            return rtti is not null;
        }

        public bool Contains(string name)
        {
            foreach (var scope in this.scopes.Reverse())
            {
                if (scope.Symbols.ContainsKey(name))
                {
                    return true;
                }
            }
            return false;
        }

        public RTTI? LookUpInCurrentScope(string name)
        {
            return scopes.Last?.Value.Symbols.TryGetValue(name, out var rtti)
                    ?? throw new InternalErrorException("The scope stack is unexpextedly empty.")
                ? rtti : null;
        }

        public bool TryLookUpInCurrentScope(string name, [MaybeNullWhen(false), NotNullWhen(true)] out RTTI? rtti)
        {
            rtti = LookUpInCurrentScope(name);
            return rtti is not null;
        }

        public bool ContainsInCurrentScope(string name)
        {
            return scopes.Last?.Value.Symbols.ContainsKey(name)
                ?? throw new InternalErrorException("The scope stack is unexpextedly empty.");
        }

        public bool TryAdd(string name, RTTI rtti)
        {
            var currentScope = scopes.Last ?? throw new InternalErrorException("The scope stack is unexpextedly empty.");
            if (currentScope.Value.Symbols.ContainsKey(name))
            {
                return false;
            }
            currentScope.Value.Symbols.Add(name, rtti);
            return true;
        }

        public void Add(string name, RTTI rtti)
        {
            if (!TryAdd(name, rtti))
            {
                throw new InternalErrorException($"The symbol {name} should not be defined defined in the current scope.");
            }
        }

        public void AttachScope(Scope scope)
        {
            this.scopes.AddLast(scope);
        }

        public void DetachScope()
        {
            if (this.scopes.Count == 1)
            {
                throw new InternalErrorException($"Trying to pop the global scope.");
            }

            if (this.scopes.Count <= 0)
            {
                throw new InternalErrorException($"The count of the scope stack is unexpextedly {this.scopes.Count}.");
            }

            this.scopes.Last!.Value.ClearValues();
            this.scopes.RemoveLast();
        }

        public void DestroyScope()
        {
            if (this.scopes.Count == 1)
            {
                throw new InternalErrorException($"Trying to pop the global scope.");
            }

            if (this.scopes.Count <= 0)
            {
                throw new InternalErrorException($"The count of the scope stack is unexpextedly {this.scopes.Count}.");
            }

            this.scopes.Last!.Value.ClearValues();
            this.scopes.RemoveLast();
        }
    }
}
