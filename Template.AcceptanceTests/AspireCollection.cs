using Template.E2E.Tests;

namespace Template.AcceptanceTests;

[CollectionDefinition("AspireCollection")]
public class AspireCollection : ICollectionFixture<AspireTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
