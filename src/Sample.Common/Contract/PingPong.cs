namespace Sample.Common.Contract;

public record PingOne(Guid StarterId);

public record PingTwo(Guid StarterId);

public record PongOne(Guid StarterId);

public record PongTwo(Guid StarterId);
