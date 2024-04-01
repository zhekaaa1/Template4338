> [!IMPORTANT]
> This Project is not yet stable. Breaking changes may occur at any time.

<!-- replace start here -->
### MmsCore.Extensions

[![NuGet Version](https://img.shields.io/nuget/v/MmsCore.Extensions)](https://www.nuget.org/packages/MmsCore.Extensions)
[![MmsCore.Extensions NuGet Package Downloads](https://img.shields.io/nuget/dt/MmsCore.Extensions)](https://www.nuget.org/packages/MmsCore.Extensions)
![TFM](https://img.shields.io/badge/netstandard2.0-blue)
![TFM](https://img.shields.io/badge/net6.0-blue)
![TFM](https://img.shields.io/badge/net8.0-blue)

MmsCore.Extensionsは、DateTime、IEnumerable、ObservableCollection、stringクラスの拡張メソッドを提供します。日付操作、シーケンス操作、コレクション変換、文字列処理の操作が含まれます。

<!-- replace end here -->

## 🚀 Getting Started

### `DateTimeExtensions`

以下に `DateTimeExtensions.ToNextDay` の使用例を示します。

```csharp
// Arrange
DateTime? today = DateTime.Today;
// Act
var tomorrow = today.ToNextDay();
// Assert
Assert.Equal(DateTime.Today.AddDays(1), tomorrow);
```

以下に `DateTimeExtensions.ToNextMinute` の使用例を示します。

```csharp
// Arrange
DateTime? now = DateTime.Now;
// Act
var nextMinute = now.ToNextMinute();
// Assert
Assert.Equal(DateTime.Now.AddMinutes(1).Minute, nextMinute?.Minute);
```

以下に `DateTimeExtensions.ToFirstDayInMonth` の使用例を示します。

```csharp
// Arrange
var input = new DateTime(2024, 2, 15);
// Act
var result = input.ToFirstDayInMonth();
// Assert
Assert.Equal(new DateTime(2024, 2, 1), result);
```

以下に `DateTimeExtensions.ToLastDayInMonth` の使用例を示します。

```csharp
// Arrange
var input = new DateTime(2024, 2, 15);
// Act
var result = input.ToLastDayInMonth();
// Assert
Assert.Equal(new DateTime(2024, 2, 29), result);
```

以下に `DateTimeExtensions.ToExceptedMilliseconds` の使用例を示します。

```csharp
// Arrange
var now = DateTime.Now;
// Act
var result = now.ToExceptedMilliseconds();
// Assert
Assert.Equal(0, result.Millisecond);
```

以下に `DateTimeExtensions.ToExceptedSeconds` の使用例を示します。

```csharp
// Arrange
var now = DateTime.Now;
// Act
var result = now.ToExceptedSeconds();
// Assert
Assert.Equal(0, result.Second);
Assert.Equal(0, result.Millisecond);
```

### `EnumerableExtensions`

以下に `EnumerableExtensions.DistinctBy` の使用例を示します。

```csharp
// Arrange
List<int> source = [1, 2, 2, 3, 3, 3, 4, 4, 4, 4];
List<int> expected = [1, 2, 3, 4];
// Act
var result = source.DistinctBy(x => x).ToList();
// Assert
Assert.Equal(expected, result);
```

以下に `EnumerableExtensions.ForEach` の使用例を示します。

```csharp
// Arrange
var source = Enumerable.Range(1, 5);
var results = new List<int>();
// Act
source.ForEach(item => { results.Add(item * 2); });
// Assert
Assert.Equal([2, 4, 6, 8, 10], results);
```

以下に `EnumerableExtensions.LeftOuterJoin` の使用例を示します。

```csharp
// Arrange
var outer = new List<int?> { 1, 2, 3 };
var inner = new List<int?> { 2, 3, 4 };
// Act
var result = outer.LeftOuterJoin(
    inner,
    x => x,
    y => y,
    (x, y) => new { Outer = x, Inner = y }).ToList();
// Assert
// 以下の結果が期待されます:
// { Outer = 1, Inner = null }
// { Outer = 2, Inner = 2 }
// { Outer = 3, Inner = 3 }
Assert.Equal(3, result.Count);
Assert.Equal(1, result[0].Outer);
Assert.Null(result[0].Inner);
Assert.Equal(2, result[1].Outer);
Assert.Equal(2, result[1].Inner);
Assert.Equal(3, result[2].Outer);
Assert.Equal(3, result[2].Inner);
```

以下に `EnumerableExtensions.FullOuterJoin` の使用例を示します。

```csharp
// Arrange
var left = new List<int?> { 1, 2, 3 };
var right = new List<int?> { 2, 3, 4 };
// Act
var result = left.FullOuterJoin(
    right,
    x => x,
    y => y,
    (_, x, y) => new { Left = x, Right = y }).ToList();
// Assert
// 以下の結果が期待されます:
// { Left = 1, Right = null }
// { Left = 2, Right = 2 }
// { Left = 3, Right = 3 }
// { Left = null, Right = 4 }
Assert.Equal(4, result.Count);
Assert.Equal(1, result[0].Left);
Assert.Null(result[0].Right);
Assert.Equal(2, result[1].Left);
Assert.Equal(2, result[1].Right);
Assert.Equal(3, result[2].Left);
Assert.Equal(3, result[2].Right);
Assert.Null(result[3].Left);
Assert.Equal(4, result[3].Right);
```

以下に `EnumerableExtensions.SequenceIsContiguous` の使用例を示します。

```csharp
// Arrange
List<int> numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
IEnumerable<int> sequence1 = [3, 4, 5];
IEnumerable<int> sequence2 = [3, 5, 6];
        
// Act
var isContiguous1 = numbers.SequenceIsContiguous(sequence1);
var isContiguous2 = numbers.SequenceIsContiguous(sequence2);
        
// Assert
Assert.True(isContiguous1);
Assert.False(isContiguous2);
```

以下に `EnumerableExtensions.WithIndex` の使用例を示します。

```csharp
var data = new List<string> { "One", "Two", "Three" };
var result = data.WithIndex().ToList();
        
Assert.Equal(3, result.Count);
        
Assert.Equal(0, result[0].Index);
Assert.Equal("One", result[0].Item);

Assert.Equal(1, result[1].Index);
Assert.Equal("Two", result[1].Item);
        
Assert.Equal(2, result[2].Index);
Assert.Equal("Three", result[2].Item);
```

以下に `EnumerableExtensions.WithoutIndex` の使用例を示します。

```csharp
var data = new List<string> { "One", "Two", "Three" }.WithIndex();
var result = data.WithoutIndex().ToList();
        
Assert.Equal(3, result.Count);
Assert.Equal("One", result[0]);
Assert.Equal("Two", result[1]);
Assert.Equal("Three", result[2]);
```
