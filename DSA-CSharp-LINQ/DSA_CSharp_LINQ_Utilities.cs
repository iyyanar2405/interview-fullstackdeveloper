using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSA_CSharp_LINQ_Utilities
{
    // ================================
    // CHAPTER 1: ARRAYS & STRINGS
    // ================================
    
    public static class ArrayUtilities
    {
        // Two Sum Problem
        public static int[] TwoSum(int[] nums, int target)
        {
            var dict = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                int complement = target - nums[i];
                if (dict.ContainsKey(complement))
                    return new int[] { dict[complement], i };
                dict[nums[i]] = i;
            }
            return new int[0];
        }

        // Maximum Subarray (Kadane's Algorithm)
        public static int MaxSubArray(int[] nums)
        {
            int maxSoFar = nums[0];
            int maxEndingHere = nums[0];
            
            for (int i = 1; i < nums.Length; i++)
            {
                maxEndingHere = Math.Max(nums[i], maxEndingHere + nums[i]);
                maxSoFar = Math.Max(maxSoFar, maxEndingHere);
            }
            return maxSoFar;
        }

        // Rotate Array Right by K positions
        public static void RotateArray(int[] nums, int k)
        {
            k %= nums.Length;
            Array.Reverse(nums);
            Array.Reverse(nums, 0, k);
            Array.Reverse(nums, k, nums.Length - k);
        }

        // Find Missing Number (1 to n)
        public static int FindMissingNumber(int[] nums)
        {
            int n = nums.Length + 1;
            int expectedSum = n * (n + 1) / 2;
            int actualSum = nums.Sum();
            return expectedSum - actualSum;
        }

        // Remove Duplicates from Sorted Array
        public static int RemoveDuplicates(int[] nums)
        {
            if (nums.Length == 0) return 0;
            
            int i = 0;
            for (int j = 1; j < nums.Length; j++)
            {
                if (nums[j] != nums[i])
                {
                    i++;
                    nums[i] = nums[j];
                }
            }
            return i + 1;
        }

        // Merge Two Sorted Arrays
        public static int[] MergeSortedArrays(int[] nums1, int[] nums2)
        {
            return nums1.Concat(nums2).OrderBy(x => x).ToArray();
        }

        // LINQ Version of Merge
        public static int[] MergeSortedArraysLINQ(int[] nums1, int[] nums2)
        {
            return nums1.Union(nums2).OrderBy(x => x).ToArray();
        }

        // Find Intersection of Two Arrays
        public static int[] Intersection(int[] nums1, int[] nums2)
        {
            return nums1.Intersect(nums2).ToArray();
        }

        // Buy and Sell Stock (Best Time)
        public static int MaxProfit(int[] prices)
        {
            int minPrice = int.MaxValue;
            int maxProfit = 0;
            
            foreach (int price in prices)
            {
                if (price < minPrice)
                    minPrice = price;
                else if (price - minPrice > maxProfit)
                    maxProfit = price - minPrice;
            }
            return maxProfit;
        }
    }

    public static class StringUtilities
    {
        // Check if String is Palindrome
        public static bool IsPalindrome(string s)
        {
            var cleaned = new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLower();
            return cleaned.SequenceEqual(cleaned.Reverse());
        }

        // Valid Anagram
        public static bool IsAnagram(string s, string t)
        {
            return s.OrderBy(c => c).SequenceEqual(t.OrderBy(c => c));
        }

        // Longest Substring Without Repeating Characters
        public static int LengthOfLongestSubstring(string s)
        {
            var charSet = new HashSet<char>();
            int left = 0, maxLength = 0;
            
            for (int right = 0; right < s.Length; right++)
            {
                while (charSet.Contains(s[right]))
                {
                    charSet.Remove(s[left]);
                    left++;
                }
                charSet.Add(s[right]);
                maxLength = Math.Max(maxLength, right - left + 1);
            }
            return maxLength;
        }

        // Group Anagrams
        public static IList<IList<string>> GroupAnagrams(string[] strs)
        {
            return strs.GroupBy(s => string.Concat(s.OrderBy(c => c)))
                      .Select(g => g.ToList())
                      .Cast<IList<string>>()
                      .ToList();
        }

        // Reverse Words in String
        public static string ReverseWords(string s)
        {
            return string.Join(" ", s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse());
        }

        // First Non-Repeating Character
        public static int FirstUniqChar(string s)
        {
            var charCount = s.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            for (int i = 0; i < s.Length; i++)
            {
                if (charCount[s[i]] == 1)
                    return i;
            }
            return -1;
        }
    }

    // ================================
    // CHAPTER 2: LINKED LISTS
    // ================================
    
    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }

    public static class LinkedListUtilities
    {
        // Reverse Linked List
        public static ListNode ReverseList(ListNode head)
        {
            ListNode prev = null;
            ListNode current = head;
            
            while (current != null)
            {
                ListNode nextTemp = current.next;
                current.next = prev;
                prev = current;
                current = nextTemp;
            }
            return prev;
        }

        // Detect Cycle in Linked List
        public static bool HasCycle(ListNode head)
        {
            if (head == null || head.next == null) return false;
            
            ListNode slow = head;
            ListNode fast = head.next;
            
            while (slow != fast)
            {
                if (fast == null || fast.next == null) return false;
                slow = slow.next;
                fast = fast.next.next;
            }
            return true;
        }

        // Merge Two Sorted Lists
        public static ListNode MergeTwoLists(ListNode list1, ListNode list2)
        {
            var dummy = new ListNode(0);
            var current = dummy;
            
            while (list1 != null && list2 != null)
            {
                if (list1.val <= list2.val)
                {
                    current.next = list1;
                    list1 = list1.next;
                }
                else
                {
                    current.next = list2;
                    list2 = list2.next;
                }
                current = current.next;
            }
            
            current.next = list1 ?? list2;
            return dummy.next;
        }

        // Find Middle of Linked List
        public static ListNode FindMiddle(ListNode head)
        {
            ListNode slow = head;
            ListNode fast = head;
            
            while (fast != null && fast.next != null)
            {
                slow = slow.next;
                fast = fast.next.next;
            }
            return slow;
        }

        // Remove Nth Node From End
        public static ListNode RemoveNthFromEnd(ListNode head, int n)
        {
            var dummy = new ListNode(0) { next = head };
            ListNode first = dummy;
            ListNode second = dummy;
            
            for (int i = 1; i <= n + 1; i++)
                first = first.next;
            
            while (first != null)
            {
                first = first.next;
                second = second.next;
            }
            
            second.next = second.next.next;
            return dummy.next;
        }

        // Convert Linked List to Array (for LINQ operations)
        public static int[] ToArray(ListNode head)
        {
            var result = new List<int>();
            while (head != null)
            {
                result.Add(head.val);
                head = head.next;
            }
            return result.ToArray();
        }

        // Create Linked List from Array
        public static ListNode FromArray(int[] arr)
        {
            if (arr.Length == 0) return null;
            
            var head = new ListNode(arr[0]);
            var current = head;
            
            for (int i = 1; i < arr.Length; i++)
            {
                current.next = new ListNode(arr[i]);
                current = current.next;
            }
            return head;
        }
    }

    // ================================
    // CHAPTER 3: STACKS & QUEUES
    // ================================
    
    public static class StackQueueUtilities
    {
        // Valid Parentheses
        public static bool IsValidParentheses(string s)
        {
            var stack = new Stack<char>();
            var pairs = new Dictionary<char, char> { { ')', '(' }, { '}', '{' }, { ']', '[' } };
            
            foreach (char c in s)
            {
                if (pairs.ContainsValue(c))
                {
                    stack.Push(c);
                }
                else if (pairs.ContainsKey(c))
                {
                    if (stack.Count == 0 || stack.Pop() != pairs[c])
                        return false;
                }
            }
            return stack.Count == 0;
        }

        // Next Greater Element
        public static int[] NextGreaterElement(int[] nums)
        {
            var result = new int[nums.Length];
            var stack = new Stack<int>();
            
            for (int i = nums.Length - 1; i >= 0; i--)
            {
                while (stack.Count > 0 && stack.Peek() <= nums[i])
                    stack.Pop();
                
                result[i] = stack.Count == 0 ? -1 : stack.Peek();
                stack.Push(nums[i]);
            }
            return result;
        }

        // Implement Queue using Stacks
        public class MyQueue
        {
            private Stack<int> input = new Stack<int>();
            private Stack<int> output = new Stack<int>();
            
            public void Push(int x) => input.Push(x);
            
            public int Pop()
            {
                Peek();
                return output.Pop();
            }
            
            public int Peek()
            {
                if (output.Count == 0)
                    while (input.Count > 0)
                        output.Push(input.Pop());
                return output.Peek();
            }
            
            public bool Empty() => input.Count == 0 && output.Count == 0;
        }

        // Sliding Window Maximum
        public static int[] MaxSlidingWindow(int[] nums, int k)
        {
            var deque = new LinkedList<int>();
            var result = new int[nums.Length - k + 1];
            
            for (int i = 0; i < nums.Length; i++)
            {
                while (deque.Count > 0 && deque.First.Value < i - k + 1)
                    deque.RemoveFirst();
                
                while (deque.Count > 0 && nums[deque.Last.Value] < nums[i])
                    deque.RemoveLast();
                
                deque.AddLast(i);
                
                if (i >= k - 1)
                    result[i - k + 1] = nums[deque.First.Value];
            }
            return result;
        }
    }

    // ================================
    // CHAPTER 4: TREES & GRAPHS
    // ================================
    
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }

    public static class TreeUtilities
    {
        // Inorder Traversal
        public static IList<int> InorderTraversal(TreeNode root)
        {
            var result = new List<int>();
            InorderHelper(root, result);
            return result;
        }
        
        private static void InorderHelper(TreeNode node, IList<int> result)
        {
            if (node != null)
            {
                InorderHelper(node.left, result);
                result.Add(node.val);
                InorderHelper(node.right, result);
            }
        }

        // Level Order Traversal
        public static IList<IList<int>> LevelOrder(TreeNode root)
        {
            var result = new List<IList<int>>();
            if (root == null) return result;
            
            var queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            
            while (queue.Count > 0)
            {
                int levelSize = queue.Count;
                var currentLevel = new List<int>();
                
                for (int i = 0; i < levelSize; i++)
                {
                    var node = queue.Dequeue();
                    currentLevel.Add(node.val);
                    
                    if (node.left != null) queue.Enqueue(node.left);
                    if (node.right != null) queue.Enqueue(node.right);
                }
                result.Add(currentLevel);
            }
            return result;
        }

        // Maximum Depth
        public static int MaxDepth(TreeNode root)
        {
            if (root == null) return 0;
            return 1 + Math.Max(MaxDepth(root.left), MaxDepth(root.right));
        }

        // Validate Binary Search Tree
        public static bool IsValidBST(TreeNode root)
        {
            return IsValidBST(root, long.MinValue, long.MaxValue);
        }
        
        private static bool IsValidBST(TreeNode node, long minVal, long maxVal)
        {
            if (node == null) return true;
            if (node.val <= minVal || node.val >= maxVal) return false;
            return IsValidBST(node.left, minVal, node.val) && IsValidBST(node.right, node.val, maxVal);
        }

        // Lowest Common Ancestor
        public static TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
        {
            if (root == null || root == p || root == q) return root;
            
            TreeNode left = LowestCommonAncestor(root.left, p, q);
            TreeNode right = LowestCommonAncestor(root.right, p, q);
            
            return (left != null && right != null) ? root : left ?? right;
        }

        // Path Sum
        public static bool HasPathSum(TreeNode root, int targetSum)
        {
            if (root == null) return false;
            if (root.left == null && root.right == null) return root.val == targetSum;
            return HasPathSum(root.left, targetSum - root.val) || HasPathSum(root.right, targetSum - root.val);
        }
    }

    public static class GraphUtilities
    {
        // DFS Traversal
        public static void DFS(Dictionary<int, List<int>> graph, int start, HashSet<int> visited, List<int> result)
        {
            visited.Add(start);
            result.Add(start);
            
            if (graph.ContainsKey(start))
            {
                foreach (int neighbor in graph[start])
                {
                    if (!visited.Contains(neighbor))
                        DFS(graph, neighbor, visited, result);
                }
            }
        }

        // BFS Traversal
        public static List<int> BFS(Dictionary<int, List<int>> graph, int start)
        {
            var result = new List<int>();
            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            
            queue.Enqueue(start);
            visited.Add(start);
            
            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                result.Add(node);
                
                if (graph.ContainsKey(node))
                {
                    foreach (int neighbor in graph[node])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return result;
        }

        // Detect Cycle in Directed Graph
        public static bool HasCycle(Dictionary<int, List<int>> graph)
        {
            var visited = new HashSet<int>();
            var recStack = new HashSet<int>();
            
            foreach (int node in graph.Keys)
            {
                if (!visited.Contains(node))
                {
                    if (HasCycleDFS(graph, node, visited, recStack))
                        return true;
                }
            }
            return false;
        }
        
        private static bool HasCycleDFS(Dictionary<int, List<int>> graph, int node, HashSet<int> visited, HashSet<int> recStack)
        {
            visited.Add(node);
            recStack.Add(node);
            
            if (graph.ContainsKey(node))
            {
                foreach (int neighbor in graph[node])
                {
                    if (!visited.Contains(neighbor))
                    {
                        if (HasCycleDFS(graph, neighbor, visited, recStack))
                            return true;
                    }
                    else if (recStack.Contains(neighbor))
                    {
                        return true;
                    }
                }
            }
            
            recStack.Remove(node);
            return false;
        }

        // Topological Sort
        public static List<int> TopologicalSort(Dictionary<int, List<int>> graph)
        {
            var inDegree = new Dictionary<int, int>();
            var result = new List<int>();
            var queue = new Queue<int>();
            
            // Initialize in-degree
            foreach (var node in graph.Keys)
                inDegree[node] = 0;
            
            foreach (var kvp in graph)
            {
                foreach (var neighbor in kvp.Value)
                {
                    inDegree[neighbor] = inDegree.GetValueOrDefault(neighbor, 0) + 1;
                }
            }
            
            // Add nodes with 0 in-degree to queue
            foreach (var kvp in inDegree)
            {
                if (kvp.Value == 0)
                    queue.Enqueue(kvp.Key);
            }
            
            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                result.Add(node);
                
                if (graph.ContainsKey(node))
                {
                    foreach (int neighbor in graph[node])
                    {
                        inDegree[neighbor]--;
                        if (inDegree[neighbor] == 0)
                            queue.Enqueue(neighbor);
                    }
                }
            }
            
            return result;
        }
    }

    // ================================
    // CHAPTER 5: DYNAMIC PROGRAMMING
    // ================================
    
    public static class DynamicProgrammingUtilities
    {
        // Fibonacci with Memoization
        public static int Fibonacci(int n)
        {
            var memo = new Dictionary<int, int>();
            return FibHelper(n, memo);
        }
        
        private static int FibHelper(int n, Dictionary<int, int> memo)
        {
            if (n <= 1) return n;
            if (memo.ContainsKey(n)) return memo[n];
            
            memo[n] = FibHelper(n - 1, memo) + FibHelper(n - 2, memo);
            return memo[n];
        }

        // Climbing Stairs
        public static int ClimbStairs(int n)
        {
            if (n <= 2) return n;
            
            int prev2 = 1, prev1 = 2;
            for (int i = 3; i <= n; i++)
            {
                int current = prev1 + prev2;
                prev2 = prev1;
                prev1 = current;
            }
            return prev1;
        }

        // Coin Change
        public static int CoinChange(int[] coins, int amount)
        {
            var dp = new int[amount + 1];
            Array.Fill(dp, amount + 1);
            dp[0] = 0;
            
            foreach (int coin in coins)
            {
                for (int i = coin; i <= amount; i++)
                {
                    dp[i] = Math.Min(dp[i], dp[i - coin] + 1);
                }
            }
            
            return dp[amount] > amount ? -1 : dp[amount];
        }

        // Longest Increasing Subsequence
        public static int LengthOfLIS(int[] nums)
        {
            var dp = new int[nums.Length];
            Array.Fill(dp, 1);
            
            for (int i = 1; i < nums.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (nums[i] > nums[j])
                        dp[i] = Math.Max(dp[i], dp[j] + 1);
                }
            }
            
            return dp.Max();
        }

        // 0/1 Knapsack
        public static int Knapsack(int[] weights, int[] values, int capacity)
        {
            int n = weights.Length;
            var dp = new int[n + 1, capacity + 1];
            
            for (int i = 1; i <= n; i++)
            {
                for (int w = 1; w <= capacity; w++)
                {
                    if (weights[i - 1] <= w)
                        dp[i, w] = Math.Max(dp[i - 1, w], dp[i - 1, w - weights[i - 1]] + values[i - 1]);
                    else
                        dp[i, w] = dp[i - 1, w];
                }
            }
            
            return dp[n, capacity];
        }

        // Edit Distance
        public static int MinDistance(string word1, string word2)
        {
            int m = word1.Length, n = word2.Length;
            var dp = new int[m + 1, n + 1];
            
            for (int i = 0; i <= m; i++) dp[i, 0] = i;
            for (int j = 0; j <= n; j++) dp[0, j] = j;
            
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (word1[i - 1] == word2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                        dp[i, j] = 1 + Math.Min(Math.Min(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1]);
                }
            }
            
            return dp[m, n];
        }
    }

    // ================================
    // CHAPTER 6: SORTING & SEARCHING
    // ================================
    
    public static class SortingSearchingUtilities
    {
        // Quick Sort
        public static void QuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(arr, low, high);
                QuickSort(arr, low, pi - 1);
                QuickSort(arr, pi + 1, high);
            }
        }
        
        private static int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = low - 1;
            
            for (int j = low; j < high; j++)
            {
                if (arr[j] < pivot)
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
            }
            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }

        // Merge Sort
        public static void MergeSort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int mid = left + (right - left) / 2;
                MergeSort(arr, left, mid);
                MergeSort(arr, mid + 1, right);
                Merge(arr, left, mid, right);
            }
        }
        
        private static void Merge(int[] arr, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;
            
            int[] leftArr = new int[n1];
            int[] rightArr = new int[n2];
            
            Array.Copy(arr, left, leftArr, 0, n1);
            Array.Copy(arr, mid + 1, rightArr, 0, n2);
            
            int i = 0, j = 0, k = left;
            
            while (i < n1 && j < n2)
            {
                if (leftArr[i] <= rightArr[j])
                    arr[k++] = leftArr[i++];
                else
                    arr[k++] = rightArr[j++];
            }
            
            while (i < n1) arr[k++] = leftArr[i++];
            while (j < n2) arr[k++] = rightArr[j++];
        }

        // Binary Search
        public static int BinarySearch(int[] arr, int target)
        {
            int left = 0, right = arr.Length - 1;
            
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                
                if (arr[mid] == target) return mid;
                if (arr[mid] < target) left = mid + 1;
                else right = mid - 1;
            }
            return -1;
        }

        // Find First and Last Position in Sorted Array
        public static int[] SearchRange(int[] nums, int target)
        {
            return new int[] { FindFirst(nums, target), FindLast(nums, target) };
        }
        
        private static int FindFirst(int[] nums, int target)
        {
            int left = 0, right = nums.Length - 1, result = -1;
            
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (nums[mid] == target)
                {
                    result = mid;
                    right = mid - 1;
                }
                else if (nums[mid] < target)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return result;
        }
        
        private static int FindLast(int[] nums, int target)
        {
            int left = 0, right = nums.Length - 1, result = -1;
            
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (nums[mid] == target)
                {
                    result = mid;
                    left = mid + 1;
                }
                else if (nums[mid] < target)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return result;
        }

        // Kth Largest Element
        public static int FindKthLargest(int[] nums, int k)
        {
            return nums.OrderByDescending(x => x).Skip(k - 1).First();
        }

        // LINQ-based sorting operations
        public static class LinqSortingUtilities
        {
            public static int[] BubbleSortLinq(int[] arr)
            {
                return arr.OrderBy(x => x).ToArray();
            }

            public static int[] SortByFrequency(int[] arr)
            {
                return arr.GroupBy(x => x)
                          .OrderByDescending(g => g.Count())
                          .ThenBy(g => g.Key)
                          .SelectMany(g => g)
                          .ToArray();
            }

            public static int[] TopKFrequent(int[] nums, int k)
            {
                return nums.GroupBy(x => x)
                          .OrderByDescending(g => g.Count())
                          .Take(k)
                          .Select(g => g.Key)
                          .ToArray();
            }
        }
    }

    // ================================
    // CHAPTER 7: HASH TABLES & HEAPS
    // ================================
    
    public static class HashTableUtilities
    {
        // Group Anagrams using Hash
        public static IList<IList<string>> GroupAnagramsHash(string[] strs)
        {
            return strs.GroupBy(s => string.Concat(s.OrderBy(c => c)))
                      .Select(g => g.ToList())
                      .Cast<IList<string>>()
                      .ToList();
        }

        // Subarray Sum Equals K
        public static int SubarraySum(int[] nums, int k)
        {
            var sumCount = new Dictionary<int, int> { { 0, 1 } };
            int count = 0, sum = 0;
            
            foreach (int num in nums)
            {
                sum += num;
                if (sumCount.ContainsKey(sum - k))
                    count += sumCount[sum - k];
                sumCount[sum] = sumCount.GetValueOrDefault(sum, 0) + 1;
            }
            return count;
        }

        // Longest Consecutive Sequence
        public static int LongestConsecutive(int[] nums)
        {
            var numSet = new HashSet<int>(nums);
            int maxLength = 0;
            
            foreach (int num in numSet)
            {
                if (!numSet.Contains(num - 1))
                {
                    int currentNum = num;
                    int currentLength = 1;
                    
                    while (numSet.Contains(currentNum + 1))
                    {
                        currentNum++;
                        currentLength++;
                    }
                    maxLength = Math.Max(maxLength, currentLength);
                }
            }
            return maxLength;
        }

        // LRU Cache Implementation
        public class LRUCache
        {
            private readonly int capacity;
            private readonly Dictionary<int, LinkedListNode<(int key, int value)>> cache;
            private readonly LinkedList<(int key, int value)> list;
            
            public LRUCache(int capacity)
            {
                this.capacity = capacity;
                cache = new Dictionary<int, LinkedListNode<(int, int)>>();
                list = new LinkedList<(int, int)>();
            }
            
            public int Get(int key)
            {
                if (cache.TryGetValue(key, out var node))
                {
                    list.Remove(node);
                    list.AddFirst(node);
                    return node.Value.value;
                }
                return -1;
            }
            
            public void Put(int key, int value)
            {
                if (cache.TryGetValue(key, out var existingNode))
                {
                    existingNode.Value = (key, value);
                    list.Remove(existingNode);
                    list.AddFirst(existingNode);
                }
                else
                {
                    if (cache.Count >= capacity)
                    {
                        var last = list.Last;
                        list.RemoveLast();
                        cache.Remove(last.Value.key);
                    }
                    
                    var newNode = list.AddFirst((key, value));
                    cache[key] = newNode;
                }
            }
        }
    }

    public static class HeapUtilities
    {
        // Min Heap Implementation
        public class MinHeap
        {
            private List<int> heap = new List<int>();
            
            public int Count => heap.Count;
            public bool IsEmpty => heap.Count == 0;
            
            public void Insert(int value)
            {
                heap.Add(value);
                HeapifyUp(heap.Count - 1);
            }
            
            public int ExtractMin()
            {
                if (IsEmpty) throw new InvalidOperationException("Heap is empty");
                
                int min = heap[0];
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                
                if (!IsEmpty) HeapifyDown(0);
                return min;
            }
            
            private void HeapifyUp(int index)
            {
                while (index > 0)
                {
                    int parentIndex = (index - 1) / 2;
                    if (heap[index] >= heap[parentIndex]) break;
                    
                    (heap[index], heap[parentIndex]) = (heap[parentIndex], heap[index]);
                    index = parentIndex;
                }
            }
            
            private void HeapifyDown(int index)
            {
                while (true)
                {
                    int smallest = index;
                    int leftChild = 2 * index + 1;
                    int rightChild = 2 * index + 2;
                    
                    if (leftChild < heap.Count && heap[leftChild] < heap[smallest])
                        smallest = leftChild;
                    
                    if (rightChild < heap.Count && heap[rightChild] < heap[smallest])
                        smallest = rightChild;
                    
                    if (smallest == index) break;
                    
                    (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                    index = smallest;
                }
            }
        }

        // K Closest Points to Origin
        public static int[][] KClosest(int[][] points, int k)
        {
            return points.OrderBy(p => p[0] * p[0] + p[1] * p[1])
                         .Take(k)
                         .ToArray();
        }

        // Top K Frequent Elements using LINQ
        public static int[] TopKFrequentLinq(int[] nums, int k)
        {
            return nums.GroupBy(x => x)
                      .OrderByDescending(g => g.Count())
                      .Take(k)
                      .Select(g => g.Key)
                      .ToArray();
        }
    }

    // ================================
    // CHAPTER 8: ADVANCED LINQ UTILITIES
    // ================================
    
    public static class AdvancedLinqUtilities
    {
        // Sliding Window Operations
        public static IEnumerable<T[]> SlidingWindow<T>(this IEnumerable<T> source, int windowSize)
        {
            var array = source.ToArray();
            for (int i = 0; i <= array.Length - windowSize; i++)
            {
                yield return array.Skip(i).Take(windowSize).ToArray();
            }
        }

        // Chunk array into groups
        public static IEnumerable<T[]> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source.Select((value, index) => new { Index = index, Value = value })
                         .GroupBy(x => x.Index / chunkSize)
                         .Select(g => g.Select(x => x.Value).ToArray());
        }

        // Running Total/Cumulative Sum
        public static IEnumerable<int> RunningTotal(this IEnumerable<int> source)
        {
            int sum = 0;
            return source.Select(x => sum += x);
        }

        // Find all permutations
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items)
        {
            if (!items.Any()) return new[] { Enumerable.Empty<T>() };
            
            return items.SelectMany((item, index) =>
                GetPermutations(items.Where((_, i) => i != index))
                    .Select(permutation => new[] { item }.Concat(permutation)));
        }

        // Cartesian Product
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(result, (current, sequence) =>
                current.SelectMany(_ => sequence, (acc, item) => acc.Concat(new[] { item })));
        }

        // Most frequent element
        public static T MostFrequent<T>(this IEnumerable<T> source)
        {
            return source.GroupBy(x => x)
                         .OrderByDescending(g => g.Count())
                         .First()
                         .Key;
        }

        // Flatten nested collections
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
        {
            return source.SelectMany(x => x);
        }

        // Binary operations on collections
        public static IEnumerable<TResult> ZipWith<T1, T2, TResult>(
            this IEnumerable<T1> first,
            IEnumerable<T2> second,
            Func<T1, T2, TResult> func)
        {
            return first.Zip(second, func);
        }

        // Statistical operations
        public static class Statistics
        {
            public static double Median(this IEnumerable<double> source)
            {
                var sorted = source.OrderBy(x => x).ToArray();
                int count = sorted.Length;
                
                if (count % 2 == 0)
                    return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
                else
                    return sorted[count / 2];
            }

            public static double Mode(this IEnumerable<double> source)
            {
                return source.GroupBy(x => x)
                             .OrderByDescending(g => g.Count())
                             .First()
                             .Key;
            }

            public static double StandardDeviation(this IEnumerable<double> source)
            {
                var array = source.ToArray();
                double mean = array.Average();
                double variance = array.Select(x => Math.Pow(x - mean, 2)).Average();
                return Math.Sqrt(variance);
            }
        }
    }

    // ================================
    // CHAPTER 9: SAMPLE USAGE & TESTING
    // ================================
    
    public static class DSAUsageExamples
    {
        public static void RunAllExamples()
        {
            Console.WriteLine("=== DSA C# LINQ Utilities Examples ===\n");

            // Array Examples
            Console.WriteLine("1. Array Operations:");
            int[] nums = { 2, 7, 11, 15 };
            var twoSumResult = ArrayUtilities.TwoSum(nums, 9);
            Console.WriteLine($"Two Sum Result: [{string.Join(", ", twoSumResult)}]");
            
            int[] maxSubArray = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
            Console.WriteLine($"Max Subarray Sum: {ArrayUtilities.MaxSubArray(maxSubArray)}");

            // String Examples
            Console.WriteLine("\n2. String Operations:");
            Console.WriteLine($"Is 'racecar' palindrome: {StringUtilities.IsPalindrome("racecar")}");
            Console.WriteLine($"Are 'listen' and 'silent' anagrams: {StringUtilities.IsAnagram("listen", "silent")}");

            // Linked List Examples
            Console.WriteLine("\n3. Linked List Operations:");
            var list = LinkedListUtilities.FromArray(new[] { 1, 2, 3, 4, 5 });
            var reversed = LinkedListUtilities.ReverseList(list);
            Console.WriteLine($"Reversed list: [{string.Join(", ", LinkedListUtilities.ToArray(reversed))}]");

            // Tree Examples
            Console.WriteLine("\n4. Tree Operations:");
            var root = new TreeNode(3)
            {
                left = new TreeNode(9),
                right = new TreeNode(20) { left = new TreeNode(15), right = new TreeNode(7) }
            };
            Console.WriteLine($"Tree max depth: {TreeUtilities.MaxDepth(root)}");

            // DP Examples
            Console.WriteLine("\n5. Dynamic Programming:");
            Console.WriteLine($"Fibonacci(10): {DynamicProgrammingUtilities.Fibonacci(10)}");
            Console.WriteLine($"Climb stairs (n=5): {DynamicProgrammingUtilities.ClimbStairs(5)}");

            // LINQ Examples
            Console.WriteLine("\n6. Advanced LINQ:");
            var numbers = Enumerable.Range(1, 10).ToArray();
            var slidingWindows = numbers.SlidingWindow(3).Take(3);
            Console.WriteLine("Sliding windows of size 3:");
            foreach (var window in slidingWindows)
            {
                Console.WriteLine($"[{string.Join(", ", window)}]");
            }

            var runningTotal = numbers.Take(5).RunningTotal();
            Console.WriteLine($"Running total: [{string.Join(", ", runningTotal)}]");

            // Statistical operations
            var data = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Console.WriteLine($"Median: {data.Median()}");
            Console.WriteLine($"Standard Deviation: {data.StandardDeviation():F2}");
        }
    }
}

// Usage in your main program:
/*
class Program
{
    static void Main(string[] args)
    {
        DSA_CSharp_LINQ_Utilities.DSAUsageExamples.RunAllExamples();
        
        // Individual utility usage examples:
        var result = DSA_CSharp_LINQ_Utilities.ArrayUtilities.TwoSum(new[] { 2, 7, 11, 15 }, 9);
        var palindrome = DSA_CSharp_LINQ_Utilities.StringUtilities.IsPalindrome("racecar");
        var fibonacci = DSA_CSharp_LINQ_Utilities.DynamicProgrammingUtilities.Fibonacci(10);
        
        Console.WriteLine($"Two Sum: [{string.Join(", ", result)}]");
        Console.WriteLine($"Is Palindrome: {palindrome}");
        Console.WriteLine($"Fibonacci: {fibonacci}");
    }
}
*/