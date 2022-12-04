﻿using IdGen;

namespace IGroceryStore.Shared.Services;
public interface ISnowflakeService
{
    public ulong GenerateId();
}

public class SnowflakeService : ISnowflakeService
{
    private readonly IdGenerator _generator;
    public SnowflakeService(/*IConfiguration configuration*/)
    {
        // Let's say we take jan 17st 2022 as our epoch
        var epoch = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Local);

        // Create an ID with 45 bits for timestamp, 2 for generator-id
        // and 16 for sequence
        var structure = new IdStructure(45, 2, 16);

        // Prepare options
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

        // Create an IdGenerator with it's generator-id set to 0, our custom epoch
        // and id-structure
        _generator = new IdGenerator(0, options);
    }

    public ulong GenerateId()
    {
        return (ulong)_generator.CreateId();
    }
}