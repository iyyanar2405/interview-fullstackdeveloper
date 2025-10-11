"""
DSA Python Utilities - Complete Reference
All essential Data Structures and Algorithms in Python
Optimized for VS Code snippets and modern Python features
"""

from typing import List, Optional, Dict, Set, Tuple, Any, Union, Callable
from collections import defaultdict, deque, Counter
from heapq import heappush, heappop, heapify
import bisect
import math
import functools

# ================================
# CHAPTER 1: ARRAYS & STRINGS
# ================================

class ArrayUtilities:
    """Array manipulation utilities with Pythonic approaches"""
    
    @staticmethod
    def two_sum(nums: List[int], target: int) -> List[int]:
        """Find two numbers that add up to target"""
        num_map = {}
        for i, num in enumerate(nums):
            complement = target - num
            if complement in num_map:
                return [num_map[complement], i]
            num_map[num] = i
        return []
    
    @staticmethod
    def max_subarray(nums: List[int]) -> int:
        """Maximum subarray sum using Kadane's algorithm"""
        max_so_far = max_ending_here = nums[0]
        
        for i in range(1, len(nums)):
            max_ending_here = max(nums[i], max_ending_here + nums[i])
            max_so_far = max(max_so_far, max_ending_here)
        
        return max_so_far
    
    @staticmethod
    def rotate_array(nums: List[int], k: int) -> None:
        """Rotate array to the right by k steps"""
        n = len(nums)
        k %= n
        nums[:] = nums[-k:] + nums[:-k]
    
    @staticmethod
    def find_missing_number(nums: List[int]) -> int:
        """Find missing number in range [0, n]"""
        n = len(nums)
        expected_sum = n * (n + 1) // 2
        actual_sum = sum(nums)
        return expected_sum - actual_sum
    
    @staticmethod
    def remove_duplicates(nums: List[int]) -> int:
        """Remove duplicates from sorted array in-place"""
        if not nums:
            return 0
        
        i = 0
        for j in range(1, len(nums)):
            if nums[j] != nums[i]:
                i += 1
                nums[i] = nums[j]
        return i + 1
    
    @staticmethod
    def merge_sorted_arrays(nums1: List[int], nums2: List[int]) -> List[int]:
        """Merge two sorted arrays"""
        return sorted(nums1 + nums2)
    
    @staticmethod
    def intersection(nums1: List[int], nums2: List[int]) -> List[int]:
        """Find intersection of two arrays"""
        return list(set(nums1) & set(nums2))
    
    @staticmethod
    def max_profit(prices: List[int]) -> int:
        """Best time to buy and sell stock"""
        min_price = float('inf')
        max_profit = 0
        
        for price in prices:
            if price < min_price:
                min_price = price
            elif price - min_price > max_profit:
                max_profit = price - min_price
        
        return max_profit
    
    @staticmethod
    def contains_duplicate(nums: List[int]) -> bool:
        """Check if array contains duplicates"""
        return len(nums) != len(set(nums))
    
    @staticmethod
    def product_except_self(nums: List[int]) -> List[int]:
        """Product of array except self"""
        n = len(nums)
        result = [1] * n
        
        # Forward pass
        for i in range(1, n):
            result[i] = result[i - 1] * nums[i - 1]
        
        # Backward pass
        right = 1
        for i in range(n - 1, -1, -1):
            result[i] *= right
            right *= nums[i]
        
        return result
    
    @staticmethod
    def three_sum(nums: List[int]) -> List[List[int]]:
        """Find all unique triplets that sum to zero"""
        nums.sort()
        result = []
        
        for i in range(len(nums) - 2):
            if i > 0 and nums[i] == nums[i - 1]:
                continue
            
            left, right = i + 1, len(nums) - 1
            while left < right:
                current_sum = nums[i] + nums[left] + nums[right]
                if current_sum < 0:
                    left += 1
                elif current_sum > 0:
                    right -= 1
                else:
                    result.append([nums[i], nums[left], nums[right]])
                    while left < right and nums[left] == nums[left + 1]:
                        left += 1
                    while left < right and nums[right] == nums[right - 1]:
                        right -= 1
                    left += 1
                    right -= 1
        
        return result


class StringUtilities:
    """String manipulation utilities with Python string methods"""
    
    @staticmethod
    def is_palindrome(s: str) -> bool:
        """Check if string is a palindrome"""
        cleaned = ''.join(c.lower() for c in s if c.isalnum())
        return cleaned == cleaned[::-1]
    
    @staticmethod
    def is_anagram(s: str, t: str) -> bool:
        """Check if two strings are anagrams"""
        return Counter(s) == Counter(t)
    
    @staticmethod
    def length_of_longest_substring(s: str) -> int:
        """Length of longest substring without repeating characters"""
        char_set = set()
        left = max_length = 0
        
        for right in range(len(s)):
            while s[right] in char_set:
                char_set.remove(s[left])
                left += 1
            char_set.add(s[right])
            max_length = max(max_length, right - left + 1)
        
        return max_length
    
    @staticmethod
    def group_anagrams(strs: List[str]) -> List[List[str]]:
        """Group anagrams together"""
        groups = defaultdict(list)
        for s in strs:
            key = ''.join(sorted(s))
            groups[key].append(s)
        return list(groups.values())
    
    @staticmethod
    def reverse_words(s: str) -> str:
        """Reverse words in a string"""
        return ' '.join(s.split()[::-1])
    
    @staticmethod
    def first_uniq_char(s: str) -> int:
        """Find first non-repeating character"""
        char_count = Counter(s)
        for i, char in enumerate(s):
            if char_count[char] == 1:
                return i
        return -1
    
    @staticmethod
    def valid_parentheses(s: str) -> bool:
        """Check if parentheses are valid"""
        stack = []
        mapping = {')': '(', '}': '{', ']': '['}
        
        for char in s:
            if char in mapping:
                if not stack or stack.pop() != mapping[char]:
                    return False
            else:
                stack.append(char)
        
        return not stack
    
    @staticmethod
    def longest_common_prefix(strs: List[str]) -> str:
        """Find longest common prefix"""
        if not strs:
            return ""
        
        prefix = strs[0]
        for s in strs[1:]:
            while not s.startswith(prefix):
                prefix = prefix[:-1]
                if not prefix:
                    return ""
        
        return prefix
    
    @staticmethod
    def is_subsequence(s: str, t: str) -> bool:
        """Check if s is subsequence of t"""
        i = j = 0
        while i < len(s) and j < len(t):
            if s[i] == t[j]:
                i += 1
            j += 1
        return i == len(s)
    
    @staticmethod
    def longest_palindrome(s: str) -> str:
        """Find longest palindromic substring"""
        if not s:
            return ""
        
        start = max_len = 0
        
        def expand_around_center(left: int, right: int) -> int:
            while left >= 0 and right < len(s) and s[left] == s[right]:
                left -= 1
                right += 1
            return right - left - 1
        
        for i in range(len(s)):
            len1 = expand_around_center(i, i)  # odd length
            len2 = expand_around_center(i, i + 1)  # even length
            current_max = max(len1, len2)
            
            if current_max > max_len:
                max_len = current_max
                start = i - (current_max - 1) // 2
        
        return s[start:start + max_len]


# ================================
# CHAPTER 2: LINKED LISTS
# ================================

class ListNode:
    """Linked list node definition"""
    def __init__(self, val=0, next=None):
        self.val = val
        self.next = next
    
    def __repr__(self):
        return f"ListNode({self.val})"


class LinkedListUtilities:
    """Linked list manipulation utilities"""
    
    @staticmethod
    def reverse_list(head: Optional[ListNode]) -> Optional[ListNode]:
        """Reverse a linked list"""
        prev = None
        current = head
        
        while current:
            next_temp = current.next
            current.next = prev
            prev = current
            current = next_temp
        
        return prev
    
    @staticmethod
    def has_cycle(head: Optional[ListNode]) -> bool:
        """Detect cycle in linked list using Floyd's algorithm"""
        if not head or not head.next:
            return False
        
        slow = head
        fast = head.next
        
        while slow != fast:
            if not fast or not fast.next:
                return False
            slow = slow.next
            fast = fast.next.next
        
        return True
    
    @staticmethod
    def merge_two_lists(list1: Optional[ListNode], list2: Optional[ListNode]) -> Optional[ListNode]:
        """Merge two sorted linked lists"""
        dummy = ListNode(0)
        current = dummy
        
        while list1 and list2:
            if list1.val <= list2.val:
                current.next = list1
                list1 = list1.next
            else:
                current.next = list2
                list2 = list2.next
            current = current.next
        
        current.next = list1 or list2
        return dummy.next
    
    @staticmethod
    def find_middle(head: Optional[ListNode]) -> Optional[ListNode]:
        """Find middle node of linked list"""
        slow = fast = head
        
        while fast and fast.next:
            slow = slow.next
            fast = fast.next.next
        
        return slow
    
    @staticmethod
    def remove_nth_from_end(head: Optional[ListNode], n: int) -> Optional[ListNode]:
        """Remove nth node from end"""
        dummy = ListNode(0, head)
        first = second = dummy
        
        # Move first n+1 steps ahead
        for _ in range(n + 1):
            first = first.next
        
        # Move both until first reaches end
        while first:
            first = first.next
            second = second.next
        
        # Remove the node
        second.next = second.next.next
        return dummy.next
    
    @staticmethod
    def to_list(head: Optional[ListNode]) -> List[int]:
        """Convert linked list to Python list"""
        result = []
        while head:
            result.append(head.val)
            head = head.next
        return result
    
    @staticmethod
    def from_list(arr: List[int]) -> Optional[ListNode]:
        """Create linked list from Python list"""
        if not arr:
            return None
        
        head = ListNode(arr[0])
        current = head
        
        for val in arr[1:]:
            current.next = ListNode(val)
            current = current.next
        
        return head
    
    @staticmethod
    def add_two_numbers(l1: Optional[ListNode], l2: Optional[ListNode]) -> Optional[ListNode]:
        """Add two numbers represented as linked lists"""
        dummy = ListNode(0)
        current = dummy
        carry = 0
        
        while l1 or l2 or carry:
            val1 = l1.val if l1 else 0
            val2 = l2.val if l2 else 0
            total = val1 + val2 + carry
            
            carry = total // 10
            current.next = ListNode(total % 10)
            current = current.next
            
            l1 = l1.next if l1 else None
            l2 = l2.next if l2 else None
        
        return dummy.next
    
    @staticmethod
    def intersection_node(headA: ListNode, headB: ListNode) -> Optional[ListNode]:
        """Find intersection of two linked lists"""
        if not headA or not headB:
            return None
        
        a, b = headA, headB
        
        while a != b:
            a = a.next if a else headB
            b = b.next if b else headA
        
        return a


# ================================
# CHAPTER 3: STACKS & QUEUES
# ================================

class StackQueueUtilities:
    """Stack and queue related algorithms"""
    
    @staticmethod
    def next_greater_element(nums: List[int]) -> List[int]:
        """Find next greater element for each element"""
        result = [-1] * len(nums)
        stack = []
        
        for i in range(len(nums) - 1, -1, -1):
            while stack and stack[-1] <= nums[i]:
                stack.pop()
            
            if stack:
                result[i] = stack[-1]
            
            stack.append(nums[i])
        
        return result
    
    @staticmethod
    def daily_temperatures(temperatures: List[int]) -> List[int]:
        """Days until warmer temperature"""
        result = [0] * len(temperatures)
        stack = []
        
        for i, temp in enumerate(temperatures):
            while stack and temp > temperatures[stack[-1]]:
                index = stack.pop()
                result[index] = i - index
            stack.append(i)
        
        return result
    
    @staticmethod
    def largest_rectangle_histogram(heights: List[int]) -> int:
        """Largest rectangle in histogram"""
        stack = []
        max_area = 0
        
        for i, h in enumerate(heights):
            while stack and heights[stack[-1]] > h:
                height = heights[stack.pop()]
                width = i if not stack else i - stack[-1] - 1
                max_area = max(max_area, height * width)
            stack.append(i)
        
        while stack:
            height = heights[stack.pop()]
            width = len(heights) if not stack else len(heights) - stack[-1] - 1
            max_area = max(max_area, height * width)
        
        return max_area


class MyQueue:
    """Implement queue using stacks"""
    
    def __init__(self):
        self.input_stack = []
        self.output_stack = []
    
    def push(self, x: int) -> None:
        self.input_stack.append(x)
    
    def pop(self) -> int:
        self.peek()
        return self.output_stack.pop()
    
    def peek(self) -> int:
        if not self.output_stack:
            while self.input_stack:
                self.output_stack.append(self.input_stack.pop())
        return self.output_stack[-1]
    
    def empty(self) -> bool:
        return not self.input_stack and not self.output_stack


class MyStack:
    """Implement stack using queues"""
    
    def __init__(self):
        self.queue = deque()
    
    def push(self, x: int) -> None:
        self.queue.append(x)
        for _ in range(len(self.queue) - 1):
            self.queue.append(self.queue.popleft())
    
    def pop(self) -> int:
        return self.queue.popleft()
    
    def top(self) -> int:
        return self.queue[0]
    
    def empty(self) -> bool:
        return not self.queue


class MonotonicStack:
    """Monotonic stack for various problems"""
    
    @staticmethod
    def sliding_window_maximum(nums: List[int], k: int) -> List[int]:
        """Maximum in each sliding window"""
        dq = deque()
        result = []
        
        for i, num in enumerate(nums):
            # Remove elements outside window
            while dq and dq[0] < i - k + 1:
                dq.popleft()
            
            # Remove smaller elements
            while dq and nums[dq[-1]] < num:
                dq.pop()
            
            dq.append(i)
            
            if i >= k - 1:
                result.append(nums[dq[0]])
        
        return result


# ================================
# CHAPTER 4: TREES & GRAPHS
# ================================

class TreeNode:
    """Binary tree node definition"""
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right
    
    def __repr__(self):
        return f"TreeNode({self.val})"


class TreeUtilities:
    """Binary tree algorithms"""
    
    @staticmethod
    def inorder_traversal(root: Optional[TreeNode]) -> List[int]:
        """Inorder traversal (left, root, right)"""
        result = []
        
        def dfs(node):
            if node:
                dfs(node.left)
                result.append(node.val)
                dfs(node.right)
        
        dfs(root)
        return result
    
    @staticmethod
    def preorder_traversal(root: Optional[TreeNode]) -> List[int]:
        """Preorder traversal (root, left, right)"""
        if not root:
            return []
        
        result = []
        stack = [root]
        
        while stack:
            node = stack.pop()
            result.append(node.val)
            if node.right:
                stack.append(node.right)
            if node.left:
                stack.append(node.left)
        
        return result
    
    @staticmethod
    def level_order(root: Optional[TreeNode]) -> List[List[int]]:
        """Level order traversal"""
        if not root:
            return []
        
        result = []
        queue = deque([root])
        
        while queue:
            level_size = len(queue)
            level = []
            
            for _ in range(level_size):
                node = queue.popleft()
                level.append(node.val)
                
                if node.left:
                    queue.append(node.left)
                if node.right:
                    queue.append(node.right)
            
            result.append(level)
        
        return result
    
    @staticmethod
    def max_depth(root: Optional[TreeNode]) -> int:
        """Maximum depth of binary tree"""
        if not root:
            return 0
        return 1 + max(TreeUtilities.max_depth(root.left), 
                      TreeUtilities.max_depth(root.right))
    
    @staticmethod
    def is_valid_bst(root: Optional[TreeNode]) -> bool:
        """Validate binary search tree"""
        def validate(node, min_val, max_val):
            if not node:
                return True
            if node.val <= min_val or node.val >= max_val:
                return False
            return (validate(node.left, min_val, node.val) and 
                   validate(node.right, node.val, max_val))
        
        return validate(root, float('-inf'), float('inf'))
    
    @staticmethod
    def lowest_common_ancestor(root: TreeNode, p: TreeNode, q: TreeNode) -> TreeNode:
        """Find lowest common ancestor"""
        if not root or root == p or root == q:
            return root
        
        left = TreeUtilities.lowest_common_ancestor(root.left, p, q)
        right = TreeUtilities.lowest_common_ancestor(root.right, p, q)
        
        if left and right:
            return root
        return left or right
    
    @staticmethod
    def has_path_sum(root: Optional[TreeNode], target_sum: int) -> bool:
        """Check if tree has path with given sum"""
        if not root:
            return False
        if not root.left and not root.right:
            return root.val == target_sum
        return (TreeUtilities.has_path_sum(root.left, target_sum - root.val) or
                TreeUtilities.has_path_sum(root.right, target_sum - root.val))
    
    @staticmethod
    def is_symmetric(root: Optional[TreeNode]) -> bool:
        """Check if tree is symmetric"""
        def is_mirror(t1, t2):
            if not t1 and not t2:
                return True
            if not t1 or not t2:
                return False
            return (t1.val == t2.val and 
                   is_mirror(t1.right, t2.left) and 
                   is_mirror(t1.left, t2.right))
        
        return is_mirror(root, root)
    
    @staticmethod
    def diameter_of_tree(root: Optional[TreeNode]) -> int:
        """Diameter of binary tree"""
        diameter = 0
        
        def depth(node):
            nonlocal diameter
            if not node:
                return 0
            
            left = depth(node.left)
            right = depth(node.right)
            diameter = max(diameter, left + right)
            
            return max(left, right) + 1
        
        depth(root)
        return diameter
    
    @staticmethod
    def serialize(root: Optional[TreeNode]) -> str:
        """Serialize tree to string"""
        def preorder(node):
            if node:
                vals.append(str(node.val))
                preorder(node.left)
                preorder(node.right)
            else:
                vals.append('#')
        
        vals = []
        preorder(root)
        return ','.join(vals)
    
    @staticmethod
    def deserialize(data: str) -> Optional[TreeNode]:
        """Deserialize string to tree"""
        def build():
            val = next(vals)
            if val == '#':
                return None
            node = TreeNode(int(val))
            node.left = build()
            node.right = build()
            return node
        
        vals = iter(data.split(','))
        return build()


class GraphUtilities:
    """Graph algorithms"""
    
    @staticmethod
    def dfs(graph: Dict[int, List[int]], start: int) -> List[int]:
        """Depth-first search"""
        visited = set()
        result = []
        
        def dfs_helper(node):
            visited.add(node)
            result.append(node)
            
            for neighbor in graph.get(node, []):
                if neighbor not in visited:
                    dfs_helper(neighbor)
        
        dfs_helper(start)
        return result
    
    @staticmethod
    def bfs(graph: Dict[int, List[int]], start: int) -> List[int]:
        """Breadth-first search"""
        visited = {start}
        queue = deque([start])
        result = []
        
        while queue:
            node = queue.popleft()
            result.append(node)
            
            for neighbor in graph.get(node, []):
                if neighbor not in visited:
                    visited.add(neighbor)
                    queue.append(neighbor)
        
        return result
    
    @staticmethod
    def has_cycle_directed(graph: Dict[int, List[int]]) -> bool:
        """Detect cycle in directed graph"""
        WHITE, GRAY, BLACK = 0, 1, 2
        color = defaultdict(lambda: WHITE)
        
        def dfs(node):
            if color[node] == GRAY:
                return True
            if color[node] == BLACK:
                return False
            
            color[node] = GRAY
            for neighbor in graph.get(node, []):
                if dfs(neighbor):
                    return True
            color[node] = BLACK
            return False
        
        for node in graph:
            if color[node] == WHITE:
                if dfs(node):
                    return True
        return False
    
    @staticmethod
    def topological_sort(graph: Dict[int, List[int]]) -> List[int]:
        """Topological sort using Kahn's algorithm"""
        in_degree = defaultdict(int)
        all_nodes = set(graph.keys())
        
        for node in graph:
            for neighbor in graph[node]:
                in_degree[neighbor] += 1
                all_nodes.add(neighbor)
        
        queue = deque([node for node in all_nodes if in_degree[node] == 0])
        result = []
        
        while queue:
            node = queue.popleft()
            result.append(node)
            
            for neighbor in graph.get(node, []):
                in_degree[neighbor] -= 1
                if in_degree[neighbor] == 0:
                    queue.append(neighbor)
        
        return result
    
    @staticmethod
    def num_islands(grid: List[List[str]]) -> int:
        """Count number of islands"""
        if not grid or not grid[0]:
            return 0
        
        rows, cols = len(grid), len(grid[0])
        count = 0
        
        def dfs(r, c):
            if (r < 0 or c < 0 or r >= rows or c >= cols or 
                grid[r][c] == '0'):
                return
            
            grid[r][c] = '0'  # Mark as visited
            dfs(r + 1, c)
            dfs(r - 1, c)
            dfs(r, c + 1)
            dfs(r, c - 1)
        
        for r in range(rows):
            for c in range(cols):
                if grid[r][c] == '1':
                    count += 1
                    dfs(r, c)
        
        return count
    
    @staticmethod
    def shortest_path_unweighted(graph: Dict[int, List[int]], start: int, end: int) -> List[int]:
        """Shortest path in unweighted graph"""
        if start == end:
            return [start]
        
        visited = {start}
        queue = deque([(start, [start])])
        
        while queue:
            node, path = queue.popleft()
            
            for neighbor in graph.get(node, []):
                if neighbor == end:
                    return path + [neighbor]
                if neighbor not in visited:
                    visited.add(neighbor)
                    queue.append((neighbor, path + [neighbor]))
        
        return []  # No path found


# ================================
# CHAPTER 5: DYNAMIC PROGRAMMING
# ================================

class DynamicProgrammingUtilities:
    """Dynamic programming algorithms"""
    
    @staticmethod
    @functools.lru_cache(maxsize=None)
    def fibonacci(n: int) -> int:
        """Fibonacci with memoization"""
        if n <= 1:
            return n
        return DynamicProgrammingUtilities.fibonacci(n - 1) + DynamicProgrammingUtilities.fibonacci(n - 2)
    
    @staticmethod
    def climb_stairs(n: int) -> int:
        """Number of ways to climb n stairs"""
        if n <= 2:
            return n
        
        prev2, prev1 = 1, 2
        for _ in range(3, n + 1):
            current = prev1 + prev2
            prev2, prev1 = prev1, current
        
        return prev1
    
    @staticmethod
    def coin_change(coins: List[int], amount: int) -> int:
        """Minimum coins to make amount"""
        dp = [float('inf')] * (amount + 1)
        dp[0] = 0
        
        for coin in coins:
            for i in range(coin, amount + 1):
                dp[i] = min(dp[i], dp[i - coin] + 1)
        
        return dp[amount] if dp[amount] != float('inf') else -1
    
    @staticmethod
    def length_of_lis(nums: List[int]) -> int:
        """Length of longest increasing subsequence"""
        if not nums:
            return 0
        
        dp = [1] * len(nums)
        
        for i in range(1, len(nums)):
            for j in range(i):
                if nums[i] > nums[j]:
                    dp[i] = max(dp[i], dp[j] + 1)
        
        return max(dp)
    
    @staticmethod
    def knapsack_01(weights: List[int], values: List[int], capacity: int) -> int:
        """0/1 Knapsack problem"""
        n = len(weights)
        dp = [[0] * (capacity + 1) for _ in range(n + 1)]
        
        for i in range(1, n + 1):
            for w in range(1, capacity + 1):
                if weights[i - 1] <= w:
                    dp[i][w] = max(
                        dp[i - 1][w],
                        dp[i - 1][w - weights[i - 1]] + values[i - 1]
                    )
                else:
                    dp[i][w] = dp[i - 1][w]
        
        return dp[n][capacity]
    
    @staticmethod
    def edit_distance(word1: str, word2: str) -> int:
        """Minimum edit distance between two strings"""
        m, n = len(word1), len(word2)
        dp = [[0] * (n + 1) for _ in range(m + 1)]
        
        # Initialize base cases
        for i in range(m + 1):
            dp[i][0] = i
        for j in range(n + 1):
            dp[0][j] = j
        
        for i in range(1, m + 1):
            for j in range(1, n + 1):
                if word1[i - 1] == word2[j - 1]:
                    dp[i][j] = dp[i - 1][j - 1]
                else:
                    dp[i][j] = 1 + min(
                        dp[i - 1][j],      # deletion
                        dp[i][j - 1],      # insertion
                        dp[i - 1][j - 1]   # substitution
                    )
        
        return dp[m][n]
    
    @staticmethod
    def house_robber(nums: List[int]) -> int:
        """Maximum money that can be robbed"""
        if not nums:
            return 0
        if len(nums) == 1:
            return nums[0]
        
        prev2, prev1 = nums[0], max(nums[0], nums[1])
        
        for i in range(2, len(nums)):
            current = max(prev1, prev2 + nums[i])
            prev2, prev1 = prev1, current
        
        return prev1
    
    @staticmethod
    def count_palindromic_substrings(s: str) -> int:
        """Count palindromic substrings"""
        count = 0
        
        def expand_around_center(left: int, right: int) -> int:
            nonlocal count
            while left >= 0 and right < len(s) and s[left] == s[right]:
                count += 1
                left -= 1
                right += 1
        
        for i in range(len(s)):
            expand_around_center(i, i)      # odd length
            expand_around_center(i, i + 1)  # even length
        
        return count
    
    @staticmethod
    def word_break(s: str, word_dict: List[str]) -> bool:
        """Check if string can be segmented into dictionary words"""
        word_set = set(word_dict)
        dp = [False] * (len(s) + 1)
        dp[0] = True
        
        for i in range(1, len(s) + 1):
            for j in range(i):
                if dp[j] and s[j:i] in word_set:
                    dp[i] = True
                    break
        
        return dp[len(s)]
    
    @staticmethod
    def unique_paths(m: int, n: int) -> int:
        """Number of unique paths in m x n grid"""
        dp = [[1] * n for _ in range(m)]
        
        for i in range(1, m):
            for j in range(1, n):
                dp[i][j] = dp[i - 1][j] + dp[i][j - 1]
        
        return dp[m - 1][n - 1]


# ================================
# CHAPTER 6: SORTING & SEARCHING
# ================================

class SortingSearchingUtilities:
    """Sorting and searching algorithms"""
    
    @staticmethod
    def quick_sort(arr: List[int]) -> List[int]:
        """Quick sort implementation"""
        if len(arr) <= 1:
            return arr
        
        pivot = arr[len(arr) // 2]
        left = [x for x in arr if x < pivot]
        middle = [x for x in arr if x == pivot]
        right = [x for x in arr if x > pivot]
        
        return (SortingSearchingUtilities.quick_sort(left) + 
               middle + 
               SortingSearchingUtilities.quick_sort(right))
    
    @staticmethod
    def merge_sort(arr: List[int]) -> List[int]:
        """Merge sort implementation"""
        if len(arr) <= 1:
            return arr
        
        mid = len(arr) // 2
        left = SortingSearchingUtilities.merge_sort(arr[:mid])
        right = SortingSearchingUtilities.merge_sort(arr[mid:])
        
        return SortingSearchingUtilities._merge(left, right)
    
    @staticmethod
    def _merge(left: List[int], right: List[int]) -> List[int]:
        """Merge two sorted arrays"""
        result = []
        i = j = 0
        
        while i < len(left) and j < len(right):
            if left[i] <= right[j]:
                result.append(left[i])
                i += 1
            else:
                result.append(right[j])
                j += 1
        
        result.extend(left[i:])
        result.extend(right[j:])
        return result
    
    @staticmethod
    def binary_search(arr: List[int], target: int) -> int:
        """Binary search in sorted array"""
        left, right = 0, len(arr) - 1
        
        while left <= right:
            mid = (left + right) // 2
            
            if arr[mid] == target:
                return mid
            elif arr[mid] < target:
                left = mid + 1
            else:
                right = mid - 1
        
        return -1
    
    @staticmethod
    def search_range(nums: List[int], target: int) -> List[int]:
        """Find first and last position of target"""
        def find_first():
            left, right = 0, len(nums) - 1
            result = -1
            
            while left <= right:
                mid = (left + right) // 2
                if nums[mid] == target:
                    result = mid
                    right = mid - 1
                elif nums[mid] < target:
                    left = mid + 1
                else:
                    right = mid - 1
            
            return result
        
        def find_last():
            left, right = 0, len(nums) - 1
            result = -1
            
            while left <= right:
                mid = (left + right) // 2
                if nums[mid] == target:
                    result = mid
                    left = mid + 1
                elif nums[mid] < target:
                    left = mid + 1
                else:
                    right = mid - 1
            
            return result
        
        return [find_first(), find_last()]
    
    @staticmethod
    def find_kth_largest(nums: List[int], k: int) -> int:
        """Find kth largest element"""
        return sorted(nums, reverse=True)[k - 1]
    
    @staticmethod
    def search_rotated(nums: List[int], target: int) -> int:
        """Search in rotated sorted array"""
        left, right = 0, len(nums) - 1
        
        while left <= right:
            mid = (left + right) // 2
            
            if nums[mid] == target:
                return mid
            
            if nums[left] <= nums[mid]:  # left half is sorted
                if nums[left] <= target < nums[mid]:
                    right = mid - 1
                else:
                    left = mid + 1
            else:  # right half is sorted
                if nums[mid] < target <= nums[right]:
                    left = mid + 1
                else:
                    right = mid - 1
        
        return -1
    
    @staticmethod
    def top_k_frequent(nums: List[int], k: int) -> List[int]:
        """Find k most frequent elements"""
        count = Counter(nums)
        return [num for num, _ in count.most_common(k)]
    
    @staticmethod
    def merge_intervals(intervals: List[List[int]]) -> List[List[int]]:
        """Merge overlapping intervals"""
        if not intervals:
            return []
        
        intervals.sort(key=lambda x: x[0])
        merged = [intervals[0]]
        
        for current in intervals[1:]:
            last = merged[-1]
            if current[0] <= last[1]:
                last[1] = max(last[1], current[1])
            else:
                merged.append(current)
        
        return merged


# ================================
# CHAPTER 7: HASH TABLES & HEAPS
# ================================

class HashTableUtilities:
    """Hash table related algorithms"""
    
    @staticmethod
    def subarray_sum(nums: List[int], k: int) -> int:
        """Count subarrays with sum equal to k"""
        count = 0
        prefix_sum = 0
        sum_count = defaultdict(int)
        sum_count[0] = 1
        
        for num in nums:
            prefix_sum += num
            count += sum_count[prefix_sum - k]
            sum_count[prefix_sum] += 1
        
        return count
    
    @staticmethod
    def longest_consecutive(nums: List[int]) -> int:
        """Length of longest consecutive sequence"""
        num_set = set(nums)
        max_length = 0
        
        for num in num_set:
            if num - 1 not in num_set:  # start of sequence
                current_num = num
                current_length = 1
                
                while current_num + 1 in num_set:
                    current_num += 1
                    current_length += 1
                
                max_length = max(max_length, current_length)
        
        return max_length
    
    @staticmethod
    def find_anagrams(s: str, p: str) -> List[int]:
        """Find all anagram start indices"""
        if len(s) < len(p):
            return []
        
        result = []
        p_count = Counter(p)
        window_count = Counter()
        
        left = 0
        for right in range(len(s)):
            window_count[s[right]] += 1
            
            if right - left + 1 == len(p):
                if window_count == p_count:
                    result.append(left)
                
                window_count[s[left]] -= 1
                if window_count[s[left]] == 0:
                    del window_count[s[left]]
                left += 1
        
        return result


class LRUCache:
    """LRU Cache implementation"""
    
    def __init__(self, capacity: int):
        self.capacity = capacity
        self.cache = {}
        self.order = deque()
    
    def get(self, key: int) -> int:
        if key in self.cache:
            self.order.remove(key)
            self.order.append(key)
            return self.cache[key]
        return -1
    
    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self.order.remove(key)
        elif len(self.cache) >= self.capacity:
            oldest = self.order.popleft()
            del self.cache[oldest]
        
        self.cache[key] = value
        self.order.append(key)


class HeapUtilities:
    """Heap related algorithms"""
    
    @staticmethod
    def k_closest_points(points: List[List[int]], k: int) -> List[List[int]]:
        """K closest points to origin"""
        def distance(point):
            return point[0] ** 2 + point[1] ** 2
        
        return sorted(points, key=distance)[:k]
    
    @staticmethod
    def find_median_stream():
        """Find median from data stream"""
        class MedianFinder:
            def __init__(self):
                self.small = []  # max heap (negative values)
                self.large = []  # min heap
            
            def add_num(self, num: int) -> None:
                heappush(self.small, -num)
                heappush(self.large, -heappop(self.small))
                
                if len(self.large) > len(self.small):
                    heappush(self.small, -heappop(self.large))
            
            def find_median(self) -> float:
                if len(self.small) > len(self.large):
                    return -self.small[0]
                return (-self.small[0] + self.large[0]) / 2.0
        
        return MedianFinder
    
    @staticmethod
    def merge_k_sorted_lists(lists: List[Optional[ListNode]]) -> Optional[ListNode]:
        """Merge k sorted linked lists"""
        import heapq
        
        heap = []
        for i, node in enumerate(lists):
            if node:
                heappush(heap, (node.val, i, node))
        
        dummy = ListNode(0)
        current = dummy
        
        while heap:
            val, i, node = heappop(heap)
            current.next = node
            current = node
            
            if node.next:
                heappush(heap, (node.next.val, i, node.next))
        
        return dummy.next


# ================================
# CHAPTER 8: ADVANCED UTILITIES
# ================================

class AdvancedUtilities:
    """Advanced algorithms and utilities"""
    
    @staticmethod
    def sliding_window(arr: List[Any], window_size: int) -> List[List[Any]]:
        """Generate sliding windows"""
        return [arr[i:i + window_size] for i in range(len(arr) - window_size + 1)]
    
    @staticmethod
    def chunk_array(arr: List[Any], chunk_size: int) -> List[List[Any]]:
        """Chunk array into smaller arrays"""
        return [arr[i:i + chunk_size] for i in range(0, len(arr), chunk_size)]
    
    @staticmethod
    def running_sum(nums: List[int]) -> List[int]:
        """Calculate running sum"""
        result = []
        total = 0
        for num in nums:
            total += num
            result.append(total)
        return result
    
    @staticmethod
    def get_permutations(arr: List[Any]) -> List[List[Any]]:
        """Generate all permutations"""
        from itertools import permutations
        return [list(p) for p in permutations(arr)]
    
    @staticmethod
    def get_combinations(arr: List[Any], k: int) -> List[List[Any]]:
        """Generate all combinations of size k"""
        from itertools import combinations
        return [list(c) for c in combinations(arr, k)]
    
    @staticmethod
    def debounce(func: Callable, delay: float):
        """Debounce function decorator"""
        import threading
        import time
        
        def debounced(*args, **kwargs):
            def run():
                time.sleep(delay)
                func(*args, **kwargs)
            
            if hasattr(debounced, 'timer'):
                debounced.timer.cancel()
            
            debounced.timer = threading.Timer(delay, run)
            debounced.timer.start()
        
        return debounced
    
    @staticmethod
    def deep_clone(obj: Any) -> Any:
        """Deep clone an object"""
        import copy
        return copy.deepcopy(obj)
    
    @staticmethod
    def flatten_list(nested_list: List[Any]) -> List[Any]:
        """Flatten nested list"""
        result = []
        for item in nested_list:
            if isinstance(item, list):
                result.extend(AdvancedUtilities.flatten_list(item))
            else:
                result.append(item)
        return result
    
    @staticmethod
    def most_frequent(arr: List[Any]) -> Any:
        """Find most frequent element"""
        return Counter(arr).most_common(1)[0][0] if arr else None
    
    class Statistics:
        """Statistical utility functions"""
        
        @staticmethod
        def mean(numbers: List[float]) -> float:
            return sum(numbers) / len(numbers) if numbers else 0
        
        @staticmethod
        def median(numbers: List[float]) -> float:
            sorted_nums = sorted(numbers)
            n = len(sorted_nums)
            if n % 2 == 0:
                return (sorted_nums[n // 2 - 1] + sorted_nums[n // 2]) / 2
            return sorted_nums[n // 2]
        
        @staticmethod
        def mode(numbers: List[float]) -> List[float]:
            count = Counter(numbers)
            max_count = max(count.values()) if count else 0
            return [num for num, c in count.items() if c == max_count]
        
        @staticmethod
        def standard_deviation(numbers: List[float]) -> float:
            if len(numbers) < 2:
                return 0
            
            mean_val = AdvancedUtilities.Statistics.mean(numbers)
            variance = sum((x - mean_val) ** 2 for x in numbers) / len(numbers)
            return math.sqrt(variance)


# ================================
# CHAPTER 9: USAGE EXAMPLES
# ================================

class DSAUsageExamples:
    """Complete usage examples for all utilities"""
    
    @staticmethod
    def run_all_examples():
        print("=== DSA Python Utilities Examples ===\n")
        
        # Array Examples
        print("1. Array Operations:")
        nums = [2, 7, 11, 15]
        two_sum_result = ArrayUtilities.two_sum(nums, 9)
        print(f"Two Sum Result: {two_sum_result}")
        
        max_sub_array = [-2, 1, -3, 4, -1, 2, 1, -5, 4]
        print(f"Max Subarray Sum: {ArrayUtilities.max_subarray(max_sub_array)}")
        
        # String Examples
        print("\n2. String Operations:")
        print(f"Is 'racecar' palindrome: {StringUtilities.is_palindrome('racecar')}")
        print(f"Are 'listen' and 'silent' anagrams: {StringUtilities.is_anagram('listen', 'silent')}")
        
        # Linked List Examples
        print("\n3. Linked List Operations:")
        linked_list = LinkedListUtilities.from_list([1, 2, 3, 4, 5])
        reversed_list = LinkedListUtilities.reverse_list(linked_list)
        print(f"Reversed list: {LinkedListUtilities.to_list(reversed_list)}")
        
        # Tree Examples
        print("\n4. Tree Operations:")
        root = TreeNode(3)
        root.left = TreeNode(9)
        root.right = TreeNode(20)
        root.right.left = TreeNode(15)
        root.right.right = TreeNode(7)
        print(f"Tree max depth: {TreeUtilities.max_depth(root)}")
        
        # DP Examples
        print("\n5. Dynamic Programming:")
        print(f"Fibonacci(10): {DynamicProgrammingUtilities.fibonacci(10)}")
        print(f"Climb stairs (n=5): {DynamicProgrammingUtilities.climb_stairs(5)}")
        
        # Advanced Examples
        print("\n6. Advanced Utilities:")
        numbers = list(range(1, 11))
        sliding_windows = AdvancedUtilities.sliding_window(numbers, 3)[:3]
        print("Sliding windows of size 3:")
        for window in sliding_windows:
            print(f"  {window}")
        
        running_total = AdvancedUtilities.running_sum(numbers[:5])
        print(f"Running total: {running_total}")
        
        # Statistical operations
        data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
        print(f"Median: {AdvancedUtilities.Statistics.median(data)}")
        print(f"Standard Deviation: {AdvancedUtilities.Statistics.standard_deviation(data):.2f}")
        
        # Graph Examples
        print("\n7. Graph Operations:")
        graph = {
            0: [1, 2],
            1: [2],
            2: [0, 3],
            3: [3]
        }
        dfs_result = GraphUtilities.dfs(graph, 2)
        bfs_result = GraphUtilities.bfs(graph, 2)
        print(f"DFS from node 2: {dfs_result}")
        print(f"BFS from node 2: {bfs_result}")
        
        # Sorting Examples
        print("\n8. Sorting & Searching:")
        unsorted = [64, 34, 25, 12, 22, 11, 90]
        quick_sorted = SortingSearchingUtilities.quick_sort(unsorted.copy())
        merge_sorted = SortingSearchingUtilities.merge_sort(unsorted.copy())
        print(f"Quick Sort: {quick_sorted}")
        print(f"Merge Sort: {merge_sorted}")
        
        sorted_array = [1, 3, 5, 7, 9, 11, 13, 15]
        target = 7
        index = SortingSearchingUtilities.binary_search(sorted_array, target)
        print(f"Binary Search for {target}: index {index}")


# ================================
# UTILITY FUNCTIONS FOR TESTING
# ================================

def benchmark_algorithm(func: Callable, *args, iterations: int = 1000) -> float:
    """Benchmark algorithm performance"""
    import time
    
    start_time = time.time()
    for _ in range(iterations):
        func(*args)
    end_time = time.time()
    
    return (end_time - start_time) / iterations


def generate_test_data(size: int, data_type: str = 'random') -> List[int]:
    """Generate test data for algorithms"""
    import random
    
    if data_type == 'random':
        return [random.randint(1, 1000) for _ in range(size)]
    elif data_type == 'sorted':
        return list(range(1, size + 1))
    elif data_type == 'reverse_sorted':
        return list(range(size, 0, -1))
    elif data_type == 'nearly_sorted':
        data = list(range(1, size + 1))
        # Swap a few elements
        for _ in range(size // 10):
            i, j = random.randint(0, size - 1), random.randint(0, size - 1)
            data[i], data[j] = data[j], data[i]
        return data
    else:
        return [1] * size  # all same


# Main execution
if __name__ == "__main__":
    DSAUsageExamples.run_all_examples()
    
    # Example of benchmarking
    test_data = generate_test_data(1000)
    quick_sort_time = benchmark_algorithm(SortingSearchingUtilities.quick_sort, test_data.copy(), 100)
    merge_sort_time = benchmark_algorithm(SortingSearchingUtilities.merge_sort, test_data.copy(), 100)
    
    print(f"\nBenchmark Results (1000 elements, 100 iterations):")
    print(f"Quick Sort: {quick_sort_time:.6f} seconds per call")
    print(f"Merge Sort: {merge_sort_time:.6f} seconds per call")


"""
Usage Examples in your Python projects:

# Import specific utilities
from DSA_Python_Utilities import ArrayUtilities, StringUtilities, TreeUtilities

# Or import everything
from DSA_Python_Utilities import *

# Usage examples:
result = ArrayUtilities.two_sum([2, 7, 11, 15], 9)
is_palindrome = StringUtilities.is_palindrome('racecar')
fibonacci_result = DynamicProgrammingUtilities.fibonacci(10)

# Run all examples
DSAUsageExamples.run_all_examples()

# Create data structures
lru_cache = LRUCache(2)
lru_cache.put(1, 1)
lru_cache.put(2, 2)
print(lru_cache.get(1))  # Returns 1

# Use advanced utilities
data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
windows = AdvancedUtilities.sliding_window(data, 3)
stats = AdvancedUtilities.Statistics.mean(data)

# Tree operations
root = TreeNode(1)
root.left = TreeNode(2)
root.right = TreeNode(3)
max_depth = TreeUtilities.max_depth(root)
"""