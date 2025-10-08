# Templates & Checklists — DSA in Python

## Solution Template (Python)
```python
from typing import *

def solve(input_data: Any) -> Any:
    # 1) parse
    # 2) compute
    # 3) return
    pass

if __name__ == "__main__":
    # optional: simple manual tests
    pass
```

## Complexity Checklist
- Time: dominant loops/nested loops/log factors
- Space: auxiliary structures, recursion depth
- Best/average/worst where helpful

## Testing Checklist
- Happy path + edge cases (empty, single, extremes)
- Invalid inputs gracefully handled or documented
- Randomized tests when possible

## Patterns Cheat Sheet
- Sliding window, two pointers, prefix sums
- Binary search on answer, meet-in-the-middle
- BFS/DFS, topological sort, Dijkstra
- DP: knapsack, LIS, grid, partitions

## PyTest Parametrize Example
```python
import pytest
from solution import solve

@pytest.mark.parametrize("input_data,expected", [
    ("1 2\n", 3),
    ("10 -3\n", 7),
])
def test_solve(input_data, expected):
    assert solve(input_data) == expected
```
