/**
 * DSA TypeScript Utilities - Complete Reference
 * All essential Data Structures and Algorithms in TypeScript
 * Optimized for VS Code snippets and modern TypeScript features
 */

// ================================
// CHAPTER 1: ARRAYS & STRINGS
// ================================

export namespace ArrayUtilities {
    // Two Sum Problem
    export function twoSum(nums: number[], target: number): number[] {
        const map = new Map<number, number>();
        for (let i = 0; i < nums.length; i++) {
            const complement = target - nums[i];
            if (map.has(complement)) {
                return [map.get(complement)!, i];
            }
            map.set(nums[i], i);
        }
        return [];
    }

    // Maximum Subarray (Kadane's Algorithm)
    export function maxSubArray(nums: number[]): number {
        let maxSoFar = nums[0];
        let maxEndingHere = nums[0];
        
        for (let i = 1; i < nums.length; i++) {
            maxEndingHere = Math.max(nums[i], maxEndingHere + nums[i]);
            maxSoFar = Math.max(maxSoFar, maxEndingHere);
        }
        return maxSoFar;
    }

    // Rotate Array Right by K positions
    export function rotateArray(nums: number[], k: number): void {
        k %= nums.length;
        reverse(nums, 0, nums.length - 1);
        reverse(nums, 0, k - 1);
        reverse(nums, k, nums.length - 1);
    }

    function reverse(nums: number[], start: number, end: number): void {
        while (start < end) {
            [nums[start], nums[end]] = [nums[end], nums[start]];
            start++;
            end--;
        }
    }

    // Find Missing Number (1 to n)
    export function findMissingNumber(nums: number[]): number {
        const n = nums.length + 1;
        const expectedSum = (n * (n + 1)) / 2;
        const actualSum = nums.reduce((sum, num) => sum + num, 0);
        return expectedSum - actualSum;
    }

    // Remove Duplicates from Sorted Array
    export function removeDuplicates(nums: number[]): number {
        if (nums.length === 0) return 0;
        
        let i = 0;
        for (let j = 1; j < nums.length; j++) {
            if (nums[j] !== nums[i]) {
                i++;
                nums[i] = nums[j];
            }
        }
        return i + 1;
    }

    // Merge Two Sorted Arrays
    export function mergeSortedArrays(nums1: number[], nums2: number[]): number[] {
        return [...nums1, ...nums2].sort((a, b) => a - b);
    }

    // Find Intersection of Two Arrays
    export function intersection(nums1: number[], nums2: number[]): number[] {
        const set1 = new Set(nums1);
        const set2 = new Set(nums2);
        return Array.from(set1).filter(num => set2.has(num));
    }

    // Buy and Sell Stock (Best Time)
    export function maxProfit(prices: number[]): number {
        let minPrice = Infinity;
        let maxProfit = 0;
        
        for (const price of prices) {
            if (price < minPrice) {
                minPrice = price;
            } else if (price - minPrice > maxProfit) {
                maxProfit = price - minPrice;
            }
        }
        return maxProfit;
    }

    // Contains Duplicate
    export function containsDuplicate(nums: number[]): boolean {
        return new Set(nums).size !== nums.length;
    }

    // Product of Array Except Self
    export function productExceptSelf(nums: number[]): number[] {
        const result = new Array(nums.length);
        
        // Forward pass
        result[0] = 1;
        for (let i = 1; i < nums.length; i++) {
            result[i] = result[i - 1] * nums[i - 1];
        }
        
        // Backward pass
        let right = 1;
        for (let i = nums.length - 1; i >= 0; i--) {
            result[i] *= right;
            right *= nums[i];
        }
        
        return result;
    }
}

export namespace StringUtilities {
    // Check if String is Palindrome
    export function isPalindrome(s: string): boolean {
        const cleaned = s.toLowerCase().replace(/[^a-z0-9]/g, '');
        return cleaned === cleaned.split('').reverse().join('');
    }

    // Valid Anagram
    export function isAnagram(s: string, t: string): boolean {
        if (s.length !== t.length) return false;
        
        const charCount = new Map<string, number>();
        
        for (const char of s) {
            charCount.set(char, (charCount.get(char) || 0) + 1);
        }
        
        for (const char of t) {
            if (!charCount.has(char)) return false;
            charCount.set(char, charCount.get(char)! - 1);
            if (charCount.get(char) === 0) {
                charCount.delete(char);
            }
        }
        
        return charCount.size === 0;
    }

    // Longest Substring Without Repeating Characters
    export function lengthOfLongestSubstring(s: string): number {
        const charSet = new Set<string>();
        let left = 0;
        let maxLength = 0;
        
        for (let right = 0; right < s.length; right++) {
            while (charSet.has(s[right])) {
                charSet.delete(s[left]);
                left++;
            }
            charSet.add(s[right]);
            maxLength = Math.max(maxLength, right - left + 1);
        }
        
        return maxLength;
    }

    // Group Anagrams
    export function groupAnagrams(strs: string[]): string[][] {
        const groups = new Map<string, string[]>();
        
        for (const str of strs) {
            const key = str.split('').sort().join('');
            if (!groups.has(key)) {
                groups.set(key, []);
            }
            groups.get(key)!.push(str);
        }
        
        return Array.from(groups.values());
    }

    // Reverse Words in String
    export function reverseWords(s: string): string {
        return s.trim().split(/\s+/).reverse().join(' ');
    }

    // First Non-Repeating Character
    export function firstUniqChar(s: string): number {
        const charCount = new Map<string, number>();
        
        for (const char of s) {
            charCount.set(char, (charCount.get(char) || 0) + 1);
        }
        
        for (let i = 0; i < s.length; i++) {
            if (charCount.get(s[i]) === 1) {
                return i;
            }
        }
        
        return -1;
    }

    // Valid Parentheses
    export function validParentheses(s: string): boolean {
        const stack: string[] = [];
        const pairs: Record<string, string> = { ')': '(', '}': '{', ']': '[' };
        
        for (const char of s) {
            if (['(', '{', '['].includes(char)) {
                stack.push(char);
            } else if (pairs[char]) {
                if (stack.length === 0 || stack.pop() !== pairs[char]) {
                    return false;
                }
            }
        }
        
        return stack.length === 0;
    }

    // Longest Common Prefix
    export function longestCommonPrefix(strs: string[]): string {
        if (strs.length === 0) return '';
        
        let prefix = strs[0];
        for (let i = 1; i < strs.length; i++) {
            while (strs[i].indexOf(prefix) !== 0) {
                prefix = prefix.slice(0, -1);
                if (prefix === '') return '';
            }
        }
        
        return prefix;
    }
}

// ================================
// CHAPTER 2: LINKED LISTS
// ================================

export class ListNode {
    val: number;
    next: ListNode | null = null;

    constructor(val?: number, next?: ListNode | null) {
        this.val = val ?? 0;
        this.next = next ?? null;
    }
}

export namespace LinkedListUtilities {
    // Reverse Linked List
    export function reverseList(head: ListNode | null): ListNode | null {
        let prev: ListNode | null = null;
        let current = head;
        
        while (current) {
            const nextTemp = current.next;
            current.next = prev;
            prev = current;
            current = nextTemp;
        }
        
        return prev;
    }

    // Detect Cycle in Linked List
    export function hasCycle(head: ListNode | null): boolean {
        if (!head || !head.next) return false;
        
        let slow = head;
        let fast = head.next;
        
        while (slow !== fast) {
            if (!fast || !fast.next) return false;
            slow = slow.next!;
            fast = fast.next.next;
        }
        
        return true;
    }

    // Merge Two Sorted Lists
    export function mergeTwoLists(list1: ListNode | null, list2: ListNode | null): ListNode | null {
        const dummy = new ListNode(0);
        let current = dummy;
        
        while (list1 && list2) {
            if (list1.val <= list2.val) {
                current.next = list1;
                list1 = list1.next;
            } else {
                current.next = list2;
                list2 = list2.next;
            }
            current = current.next;
        }
        
        current.next = list1 || list2;
        return dummy.next;
    }

    // Find Middle of Linked List
    export function findMiddle(head: ListNode | null): ListNode | null {
        let slow = head;
        let fast = head;
        
        while (fast && fast.next) {
            slow = slow!.next;
            fast = fast.next.next;
        }
        
        return slow;
    }

    // Remove Nth Node From End
    export function removeNthFromEnd(head: ListNode | null, n: number): ListNode | null {
        const dummy = new ListNode(0, head);
        let first: ListNode | null = dummy;
        let second: ListNode | null = dummy;
        
        for (let i = 1; i <= n + 1; i++) {
            first = first!.next;
        }
        
        while (first) {
            first = first.next;
            second = second!.next;
        }
        
        second!.next = second!.next!.next;
        return dummy.next;
    }

    // Convert Linked List to Array
    export function toArray(head: ListNode | null): number[] {
        const result: number[] = [];
        while (head) {
            result.push(head.val);
            head = head.next;
        }
        return result;
    }

    // Create Linked List from Array
    export function fromArray(arr: number[]): ListNode | null {
        if (arr.length === 0) return null;
        
        const head = new ListNode(arr[0]);
        let current = head;
        
        for (let i = 1; i < arr.length; i++) {
            current.next = new ListNode(arr[i]);
            current = current.next;
        }
        
        return head;
    }

    // Add Two Numbers (represented as linked lists)
    export function addTwoNumbers(l1: ListNode | null, l2: ListNode | null): ListNode | null {
        const dummy = new ListNode(0);
        let current = dummy;
        let carry = 0;
        
        while (l1 || l2 || carry) {
            const val1 = l1?.val || 0;
            const val2 = l2?.val || 0;
            const sum = val1 + val2 + carry;
            
            carry = Math.floor(sum / 10);
            current.next = new ListNode(sum % 10);
            current = current.next;
            
            l1 = l1?.next || null;
            l2 = l2?.next || null;
        }
        
        return dummy.next;
    }
}

// ================================
// CHAPTER 3: STACKS & QUEUES
// ================================

export namespace StackQueueUtilities {
    // Next Greater Element
    export function nextGreaterElement(nums: number[]): number[] {
        const result = new Array(nums.length);
        const stack: number[] = [];
        
        for (let i = nums.length - 1; i >= 0; i--) {
            while (stack.length > 0 && stack[stack.length - 1] <= nums[i]) {
                stack.pop();
            }
            
            result[i] = stack.length === 0 ? -1 : stack[stack.length - 1];
            stack.push(nums[i]);
        }
        
        return result;
    }

    // Daily Temperatures
    export function dailyTemperatures(temperatures: number[]): number[] {
        const result = new Array(temperatures.length).fill(0);
        const stack: number[] = [];
        
        for (let i = 0; i < temperatures.length; i++) {
            while (stack.length > 0 && temperatures[i] > temperatures[stack[stack.length - 1]]) {
                const index = stack.pop()!;
                result[index] = i - index;
            }
            stack.push(i);
        }
        
        return result;
    }

    // Implement Queue using Stacks
    export class MyQueue {
        private input: number[] = [];
        private output: number[] = [];
        
        push(x: number): void {
            this.input.push(x);
        }
        
        pop(): number {
            this.peek();
            return this.output.pop()!;
        }
        
        peek(): number {
            if (this.output.length === 0) {
                while (this.input.length > 0) {
                    this.output.push(this.input.pop()!);
                }
            }
            return this.output[this.output.length - 1];
        }
        
        empty(): boolean {
            return this.input.length === 0 && this.output.length === 0;
        }
    }

    // Implement Stack using Queues
    export class MyStack {
        private queue: number[] = [];
        
        push(x: number): void {
            this.queue.push(x);
            for (let i = 0; i < this.queue.length - 1; i++) {
                this.queue.push(this.queue.shift()!);
            }
        }
        
        pop(): number {
            return this.queue.shift()!;
        }
        
        top(): number {
            return this.queue[0];
        }
        
        empty(): boolean {
            return this.queue.length === 0;
        }
    }

    // Sliding Window Maximum
    export function maxSlidingWindow(nums: number[], k: number): number[] {
        const deque: number[] = [];
        const result: number[] = [];
        
        for (let i = 0; i < nums.length; i++) {
            while (deque.length > 0 && deque[0] < i - k + 1) {
                deque.shift();
            }
            
            while (deque.length > 0 && nums[deque[deque.length - 1]] < nums[i]) {
                deque.pop();
            }
            
            deque.push(i);
            
            if (i >= k - 1) {
                result.push(nums[deque[0]]);
            }
        }
        
        return result;
    }
}

// ================================
// CHAPTER 4: TREES & GRAPHS
// ================================

export class TreeNode {
    val: number;
    left: TreeNode | null = null;
    right: TreeNode | null = null;

    constructor(val?: number, left?: TreeNode | null, right?: TreeNode | null) {
        this.val = val ?? 0;
        this.left = left ?? null;
        this.right = right ?? null;
    }
}

export namespace TreeUtilities {
    // Inorder Traversal
    export function inorderTraversal(root: TreeNode | null): number[] {
        const result: number[] = [];
        inorderHelper(root, result);
        return result;
    }
    
    function inorderHelper(node: TreeNode | null, result: number[]): void {
        if (node) {
            inorderHelper(node.left, result);
            result.push(node.val);
            inorderHelper(node.right, result);
        }
    }

    // Preorder Traversal
    export function preorderTraversal(root: TreeNode | null): number[] {
        const result: number[] = [];
        const stack: TreeNode[] = [];
        
        if (root) stack.push(root);
        
        while (stack.length > 0) {
            const node = stack.pop()!;
            result.push(node.val);
            
            if (node.right) stack.push(node.right);
            if (node.left) stack.push(node.left);
        }
        
        return result;
    }

    // Level Order Traversal
    export function levelOrder(root: TreeNode | null): number[][] {
        if (!root) return [];
        
        const result: number[][] = [];
        const queue: TreeNode[] = [root];
        
        while (queue.length > 0) {
            const levelSize = queue.length;
            const currentLevel: number[] = [];
            
            for (let i = 0; i < levelSize; i++) {
                const node = queue.shift()!;
                currentLevel.push(node.val);
                
                if (node.left) queue.push(node.left);
                if (node.right) queue.push(node.right);
            }
            
            result.push(currentLevel);
        }
        
        return result;
    }

    // Maximum Depth
    export function maxDepth(root: TreeNode | null): number {
        if (!root) return 0;
        return 1 + Math.max(maxDepth(root.left), maxDepth(root.right));
    }

    // Validate Binary Search Tree
    export function isValidBST(root: TreeNode | null): boolean {
        return isValidBSTHelper(root, -Infinity, Infinity);
    }
    
    function isValidBSTHelper(node: TreeNode | null, minVal: number, maxVal: number): boolean {
        if (!node) return true;
        if (node.val <= minVal || node.val >= maxVal) return false;
        return isValidBSTHelper(node.left, minVal, node.val) && 
               isValidBSTHelper(node.right, node.val, maxVal);
    }

    // Lowest Common Ancestor
    export function lowestCommonAncestor(root: TreeNode | null, p: TreeNode, q: TreeNode): TreeNode | null {
        if (!root || root === p || root === q) return root;
        
        const left = lowestCommonAncestor(root.left, p, q);
        const right = lowestCommonAncestor(root.right, p, q);
        
        return (left && right) ? root : left || right;
    }

    // Path Sum
    export function hasPathSum(root: TreeNode | null, targetSum: number): boolean {
        if (!root) return false;
        if (!root.left && !root.right) return root.val === targetSum;
        return hasPathSum(root.left, targetSum - root.val) || 
               hasPathSum(root.right, targetSum - root.val);
    }

    // Symmetric Tree
    export function isSymmetric(root: TreeNode | null): boolean {
        return isMirror(root, root);
    }
    
    function isMirror(t1: TreeNode | null, t2: TreeNode | null): boolean {
        if (!t1 && !t2) return true;
        if (!t1 || !t2) return false;
        return t1.val === t2.val && 
               isMirror(t1.right, t2.left) && 
               isMirror(t1.left, t2.right);
    }

    // Diameter of Binary Tree
    export function diameterOfBinaryTree(root: TreeNode | null): number {
        let diameter = 0;
        
        function depth(node: TreeNode | null): number {
            if (!node) return 0;
            
            const left = depth(node.left);
            const right = depth(node.right);
            
            diameter = Math.max(diameter, left + right);
            return Math.max(left, right) + 1;
        }
        
        depth(root);
        return diameter;
    }
}

export namespace GraphUtilities {
    // DFS Traversal
    export function dfs(graph: Map<number, number[]>, start: number): number[] {
        const visited = new Set<number>();
        const result: number[] = [];
        
        function dfsHelper(node: number): void {
            visited.add(node);
            result.push(node);
            
            const neighbors = graph.get(node) || [];
            for (const neighbor of neighbors) {
                if (!visited.has(neighbor)) {
                    dfsHelper(neighbor);
                }
            }
        }
        
        dfsHelper(start);
        return result;
    }

    // BFS Traversal
    export function bfs(graph: Map<number, number[]>, start: number): number[] {
        const visited = new Set<number>();
        const queue: number[] = [start];
        const result: number[] = [];
        
        visited.add(start);
        
        while (queue.length > 0) {
            const node = queue.shift()!;
            result.push(node);
            
            const neighbors = graph.get(node) || [];
            for (const neighbor of neighbors) {
                if (!visited.has(neighbor)) {
                    visited.add(neighbor);
                    queue.push(neighbor);
                }
            }
        }
        
        return result;
    }

    // Detect Cycle in Directed Graph
    export function hasCycle(graph: Map<number, number[]>): boolean {
        const visited = new Set<number>();
        const recStack = new Set<number>();
        
        function hasCycleDFS(node: number): boolean {
            visited.add(node);
            recStack.add(node);
            
            const neighbors = graph.get(node) || [];
            for (const neighbor of neighbors) {
                if (!visited.has(neighbor)) {
                    if (hasCycleDFS(neighbor)) return true;
                } else if (recStack.has(neighbor)) {
                    return true;
                }
            }
            
            recStack.delete(node);
            return false;
        }
        
        for (const node of graph.keys()) {
            if (!visited.has(node)) {
                if (hasCycleDFS(node)) return true;
            }
        }
        
        return false;
    }

    // Topological Sort
    export function topologicalSort(graph: Map<number, number[]>): number[] {
        const inDegree = new Map<number, number>();
        const result: number[] = [];
        const queue: number[] = [];
        
        // Initialize in-degree
        for (const node of graph.keys()) {
            inDegree.set(node, 0);
        }
        
        for (const [node, neighbors] of graph) {
            for (const neighbor of neighbors) {
                inDegree.set(neighbor, (inDegree.get(neighbor) || 0) + 1);
            }
        }
        
        // Add nodes with 0 in-degree to queue
        for (const [node, degree] of inDegree) {
            if (degree === 0) {
                queue.push(node);
            }
        }
        
        while (queue.length > 0) {
            const node = queue.shift()!;
            result.push(node);
            
            const neighbors = graph.get(node) || [];
            for (const neighbor of neighbors) {
                inDegree.set(neighbor, inDegree.get(neighbor)! - 1);
                if (inDegree.get(neighbor) === 0) {
                    queue.push(neighbor);
                }
            }
        }
        
        return result;
    }

    // Number of Islands
    export function numIslands(grid: string[][]): number {
        if (grid.length === 0) return 0;
        
        const rows = grid.length;
        const cols = grid[0].length;
        let count = 0;
        
        function dfs(r: number, c: number): void {
            if (r < 0 || c < 0 || r >= rows || c >= cols || grid[r][c] === '0') {
                return;
            }
            
            grid[r][c] = '0'; // Mark as visited
            dfs(r + 1, c);
            dfs(r - 1, c);
            dfs(r, c + 1);
            dfs(r, c - 1);
        }
        
        for (let r = 0; r < rows; r++) {
            for (let c = 0; c < cols; c++) {
                if (grid[r][c] === '1') {
                    count++;
                    dfs(r, c);
                }
            }
        }
        
        return count;
    }
}

// ================================
// CHAPTER 5: DYNAMIC PROGRAMMING
// ================================

export namespace DynamicProgrammingUtilities {
    // Fibonacci with Memoization
    export function fibonacci(n: number): number {
        const memo = new Map<number, number>();
        
        function fibHelper(n: number): number {
            if (n <= 1) return n;
            if (memo.has(n)) return memo.get(n)!;
            
            const result = fibHelper(n - 1) + fibHelper(n - 2);
            memo.set(n, result);
            return result;
        }
        
        return fibHelper(n);
    }

    // Climbing Stairs
    export function climbStairs(n: number): number {
        if (n <= 2) return n;
        
        let prev2 = 1, prev1 = 2;
        for (let i = 3; i <= n; i++) {
            const current = prev1 + prev2;
            prev2 = prev1;
            prev1 = current;
        }
        return prev1;
    }

    // Coin Change
    export function coinChange(coins: number[], amount: number): number {
        const dp = new Array(amount + 1).fill(amount + 1);
        dp[0] = 0;
        
        for (const coin of coins) {
            for (let i = coin; i <= amount; i++) {
                dp[i] = Math.min(dp[i], dp[i - coin] + 1);
            }
        }
        
        return dp[amount] > amount ? -1 : dp[amount];
    }

    // Longest Increasing Subsequence
    export function lengthOfLIS(nums: number[]): number {
        const dp = new Array(nums.length).fill(1);
        
        for (let i = 1; i < nums.length; i++) {
            for (let j = 0; j < i; j++) {
                if (nums[i] > nums[j]) {
                    dp[i] = Math.max(dp[i], dp[j] + 1);
                }
            }
        }
        
        return Math.max(...dp);
    }

    // 0/1 Knapsack
    export function knapsack(weights: number[], values: number[], capacity: number): number {
        const n = weights.length;
        const dp = Array(n + 1).fill(null).map(() => Array(capacity + 1).fill(0));
        
        for (let i = 1; i <= n; i++) {
            for (let w = 1; w <= capacity; w++) {
                if (weights[i - 1] <= w) {
                    dp[i][w] = Math.max(
                        dp[i - 1][w],
                        dp[i - 1][w - weights[i - 1]] + values[i - 1]
                    );
                } else {
                    dp[i][w] = dp[i - 1][w];
                }
            }
        }
        
        return dp[n][capacity];
    }

    // Edit Distance
    export function minDistance(word1: string, word2: string): number {
        const m = word1.length, n = word2.length;
        const dp = Array(m + 1).fill(null).map(() => Array(n + 1).fill(0));
        
        for (let i = 0; i <= m; i++) dp[i][0] = i;
        for (let j = 0; j <= n; j++) dp[0][j] = j;
        
        for (let i = 1; i <= m; i++) {
            for (let j = 1; j <= n; j++) {
                if (word1[i - 1] === word2[j - 1]) {
                    dp[i][j] = dp[i - 1][j - 1];
                } else {
                    dp[i][j] = 1 + Math.min(
                        dp[i - 1][j],     // deletion
                        dp[i][j - 1],     // insertion
                        dp[i - 1][j - 1]  // substitution
                    );
                }
            }
        }
        
        return dp[m][n];
    }

    // House Robber
    export function rob(nums: number[]): number {
        if (nums.length === 0) return 0;
        if (nums.length === 1) return nums[0];
        
        let prev2 = nums[0];
        let prev1 = Math.max(nums[0], nums[1]);
        
        for (let i = 2; i < nums.length; i++) {
            const current = Math.max(prev1, prev2 + nums[i]);
            prev2 = prev1;
            prev1 = current;
        }
        
        return prev1;
    }

    // Palindromic Substrings
    export function countSubstrings(s: string): number {
        let count = 0;
        
        function expandAroundCenter(left: number, right: number): void {
            while (left >= 0 && right < s.length && s[left] === s[right]) {
                count++;
                left--;
                right++;
            }
        }
        
        for (let i = 0; i < s.length; i++) {
            expandAroundCenter(i, i);     // odd length palindromes
            expandAroundCenter(i, i + 1); // even length palindromes
        }
        
        return count;
    }
}

// ================================
// CHAPTER 6: SORTING & SEARCHING
// ================================

export namespace SortingSearchingUtilities {
    // Quick Sort
    export function quickSort(arr: number[]): number[] {
        if (arr.length <= 1) return arr;
        
        const pivot = arr[Math.floor(arr.length / 2)];
        const left = arr.filter(x => x < pivot);
        const middle = arr.filter(x => x === pivot);
        const right = arr.filter(x => x > pivot);
        
        return [...quickSort(left), ...middle, ...quickSort(right)];
    }

    // Merge Sort
    export function mergeSort(arr: number[]): number[] {
        if (arr.length <= 1) return arr;
        
        const mid = Math.floor(arr.length / 2);
        const left = mergeSort(arr.slice(0, mid));
        const right = mergeSort(arr.slice(mid));
        
        return merge(left, right);
    }
    
    function merge(left: number[], right: number[]): number[] {
        const result: number[] = [];
        let i = 0, j = 0;
        
        while (i < left.length && j < right.length) {
            if (left[i] <= right[j]) {
                result.push(left[i++]);
            } else {
                result.push(right[j++]);
            }
        }
        
        return result.concat(left.slice(i)).concat(right.slice(j));
    }

    // Binary Search
    export function binarySearch(arr: number[], target: number): number {
        let left = 0, right = arr.length - 1;
        
        while (left <= right) {
            const mid = Math.floor((left + right) / 2);
            
            if (arr[mid] === target) return mid;
            if (arr[mid] < target) left = mid + 1;
            else right = mid - 1;
        }
        
        return -1;
    }

    // Find First and Last Position in Sorted Array
    export function searchRange(nums: number[], target: number): number[] {
        return [findFirst(nums, target), findLast(nums, target)];
    }
    
    function findFirst(nums: number[], target: number): number {
        let left = 0, right = nums.length - 1, result = -1;
        
        while (left <= right) {
            const mid = Math.floor((left + right) / 2);
            if (nums[mid] === target) {
                result = mid;
                right = mid - 1;
            } else if (nums[mid] < target) {
                left = mid + 1;
            } else {
                right = mid - 1;
            }
        }
        
        return result;
    }
    
    function findLast(nums: number[], target: number): number {
        let left = 0, right = nums.length - 1, result = -1;
        
        while (left <= right) {
            const mid = Math.floor((left + right) / 2);
            if (nums[mid] === target) {
                result = mid;
                left = mid + 1;
            } else if (nums[mid] < target) {
                left = mid + 1;
            } else {
                right = mid - 1;
            }
        }
        
        return result;
    }

    // Kth Largest Element
    export function findKthLargest(nums: number[], k: number): number {
        return nums.sort((a, b) => b - a)[k - 1];
    }

    // Search in Rotated Sorted Array
    export function searchRotated(nums: number[], target: number): number {
        let left = 0, right = nums.length - 1;
        
        while (left <= right) {
            const mid = Math.floor((left + right) / 2);
            
            if (nums[mid] === target) return mid;
            
            if (nums[left] <= nums[mid]) {
                if (nums[left] <= target && target < nums[mid]) {
                    right = mid - 1;
                } else {
                    left = mid + 1;
                }
            } else {
                if (nums[mid] < target && target <= nums[right]) {
                    left = mid + 1;
                } else {
                    right = mid - 1;
                }
            }
        }
        
        return -1;
    }

    // Top K Frequent Elements
    export function topKFrequent(nums: number[], k: number): number[] {
        const freqMap = new Map<number, number>();
        
        for (const num of nums) {
            freqMap.set(num, (freqMap.get(num) || 0) + 1);
        }
        
        return Array.from(freqMap.entries())
            .sort((a, b) => b[1] - a[1])
            .slice(0, k)
            .map(([num]) => num);
    }
}

// ================================
// CHAPTER 7: HASH TABLES & HEAPS
// ================================

export namespace HashTableUtilities {
    // Subarray Sum Equals K
    export function subarraySum(nums: number[], k: number): number {
        const sumCount = new Map<number, number>();
        sumCount.set(0, 1);
        
        let count = 0, sum = 0;
        
        for (const num of nums) {
            sum += num;
            if (sumCount.has(sum - k)) {
                count += sumCount.get(sum - k)!;
            }
            sumCount.set(sum, (sumCount.get(sum) || 0) + 1);
        }
        
        return count;
    }

    // Longest Consecutive Sequence
    export function longestConsecutive(nums: number[]): number {
        const numSet = new Set(nums);
        let maxLength = 0;
        
        for (const num of numSet) {
            if (!numSet.has(num - 1)) {
                let currentNum = num;
                let currentLength = 1;
                
                while (numSet.has(currentNum + 1)) {
                    currentNum++;
                    currentLength++;
                }
                
                maxLength = Math.max(maxLength, currentLength);
            }
        }
        
        return maxLength;
    }

    // LRU Cache Implementation
    export class LRUCache {
        private capacity: number;
        private cache = new Map<number, number>();
        
        constructor(capacity: number) {
            this.capacity = capacity;
        }
        
        get(key: number): number {
            if (this.cache.has(key)) {
                const value = this.cache.get(key)!;
                this.cache.delete(key);
                this.cache.set(key, value);
                return value;
            }
            return -1;
        }
        
        put(key: number, value: number): void {
            if (this.cache.has(key)) {
                this.cache.delete(key);
            } else if (this.cache.size >= this.capacity) {
                const firstKey = this.cache.keys().next().value;
                this.cache.delete(firstKey);
            }
            
            this.cache.set(key, value);
        }
    }

    // Find All Anagrams in a String
    export function findAnagrams(s: string, p: string): number[] {
        const result: number[] = [];
        if (s.length < p.length) return result;
        
        const pCount = new Map<string, number>();
        const windowCount = new Map<string, number>();
        
        for (const char of p) {
            pCount.set(char, (pCount.get(char) || 0) + 1);
        }
        
        let left = 0;
        for (let right = 0; right < s.length; right++) {
            const rightChar = s[right];
            windowCount.set(rightChar, (windowCount.get(rightChar) || 0) + 1);
            
            if (right - left + 1 === p.length) {
                if (mapsEqual(pCount, windowCount)) {
                    result.push(left);
                }
                
                const leftChar = s[left];
                windowCount.set(leftChar, windowCount.get(leftChar)! - 1);
                if (windowCount.get(leftChar) === 0) {
                    windowCount.delete(leftChar);
                }
                left++;
            }
        }
        
        return result;
    }
    
    function mapsEqual(map1: Map<string, number>, map2: Map<string, number>): boolean {
        if (map1.size !== map2.size) return false;
        
        for (const [key, value] of map1) {
            if (map2.get(key) !== value) return false;
        }
        
        return true;
    }
}

export namespace HeapUtilities {
    // Min Heap Implementation
    export class MinHeap {
        private heap: number[] = [];
        
        get size(): number {
            return this.heap.length;
        }
        
        get isEmpty(): boolean {
            return this.heap.length === 0;
        }
        
        insert(value: number): void {
            this.heap.push(value);
            this.heapifyUp(this.heap.length - 1);
        }
        
        extractMin(): number {
            if (this.isEmpty) throw new Error('Heap is empty');
            
            const min = this.heap[0];
            this.heap[0] = this.heap[this.heap.length - 1];
            this.heap.pop();
            
            if (!this.isEmpty) this.heapifyDown(0);
            return min;
        }
        
        peek(): number {
            if (this.isEmpty) throw new Error('Heap is empty');
            return this.heap[0];
        }
        
        private heapifyUp(index: number): void {
            while (index > 0) {
                const parentIndex = Math.floor((index - 1) / 2);
                if (this.heap[index] >= this.heap[parentIndex]) break;
                
                [this.heap[index], this.heap[parentIndex]] = [this.heap[parentIndex], this.heap[index]];
                index = parentIndex;
            }
        }
        
        private heapifyDown(index: number): void {
            while (true) {
                let smallest = index;
                const leftChild = 2 * index + 1;
                const rightChild = 2 * index + 2;
                
                if (leftChild < this.heap.length && this.heap[leftChild] < this.heap[smallest]) {
                    smallest = leftChild;
                }
                
                if (rightChild < this.heap.length && this.heap[rightChild] < this.heap[smallest]) {
                    smallest = rightChild;
                }
                
                if (smallest === index) break;
                
                [this.heap[index], this.heap[smallest]] = [this.heap[smallest], this.heap[index]];
                index = smallest;
            }
        }
    }

    // Max Heap Implementation
    export class MaxHeap {
        private heap: number[] = [];
        
        get size(): number {
            return this.heap.length;
        }
        
        get isEmpty(): boolean {
            return this.heap.length === 0;
        }
        
        insert(value: number): void {
            this.heap.push(value);
            this.heapifyUp(this.heap.length - 1);
        }
        
        extractMax(): number {
            if (this.isEmpty) throw new Error('Heap is empty');
            
            const max = this.heap[0];
            this.heap[0] = this.heap[this.heap.length - 1];
            this.heap.pop();
            
            if (!this.isEmpty) this.heapifyDown(0);
            return max;
        }
        
        peek(): number {
            if (this.isEmpty) throw new Error('Heap is empty');
            return this.heap[0];
        }
        
        private heapifyUp(index: number): void {
            while (index > 0) {
                const parentIndex = Math.floor((index - 1) / 2);
                if (this.heap[index] <= this.heap[parentIndex]) break;
                
                [this.heap[index], this.heap[parentIndex]] = [this.heap[parentIndex], this.heap[index]];
                index = parentIndex;
            }
        }
        
        private heapifyDown(index: number): void {
            while (true) {
                let largest = index;
                const leftChild = 2 * index + 1;
                const rightChild = 2 * index + 2;
                
                if (leftChild < this.heap.length && this.heap[leftChild] > this.heap[largest]) {
                    largest = leftChild;
                }
                
                if (rightChild < this.heap.length && this.heap[rightChild] > this.heap[largest]) {
                    largest = rightChild;
                }
                
                if (largest === index) break;
                
                [this.heap[index], this.heap[largest]] = [this.heap[largest], this.heap[index]];
                index = largest;
            }
        }
    }

    // K Closest Points to Origin
    export function kClosest(points: number[][], k: number): number[][] {
        return points
            .sort((a, b) => (a[0] * a[0] + a[1] * a[1]) - (b[0] * b[0] + b[1] * b[1]))
            .slice(0, k);
    }
}

// ================================
// CHAPTER 8: ADVANCED UTILITIES
// ================================

export namespace AdvancedUtilities {
    // Sliding Window Template
    export function slidingWindow<T>(arr: T[], windowSize: number): T[][] {
        const result: T[][] = [];
        for (let i = 0; i <= arr.length - windowSize; i++) {
            result.push(arr.slice(i, i + windowSize));
        }
        return result;
    }

    // Chunk Array
    export function chunk<T>(arr: T[], chunkSize: number): T[][] {
        const result: T[][] = [];
        for (let i = 0; i < arr.length; i += chunkSize) {
            result.push(arr.slice(i, i + chunkSize));
        }
        return result;
    }

    // Running Sum/Cumulative Sum
    export function runningSum(nums: number[]): number[] {
        const result: number[] = [];
        let sum = 0;
        for (const num of nums) {
            sum += num;
            result.push(sum);
        }
        return result;
    }

    // Get All Permutations
    export function getPermutations<T>(arr: T[]): T[][] {
        if (arr.length === 0) return [[]];
        
        const result: T[][] = [];
        for (let i = 0; i < arr.length; i++) {
            const rest = arr.slice(0, i).concat(arr.slice(i + 1));
            const perms = getPermutations(rest);
            for (const perm of perms) {
                result.push([arr[i], ...perm]);
            }
        }
        return result;
    }

    // Get All Combinations
    export function getCombinations<T>(arr: T[], k: number): T[][] {
        if (k === 0) return [[]];
        if (arr.length === 0) return [];
        
        const [first, ...rest] = arr;
        const withFirst = getCombinations(rest, k - 1).map(combo => [first, ...combo]);
        const withoutFirst = getCombinations(rest, k);
        
        return [...withFirst, ...withoutFirst];
    }

    // Debounce Function
    export function debounce<T extends (...args: any[]) => any>(
        func: T,
        delay: number
    ): (...args: Parameters<T>) => void {
        let timeoutId: NodeJS.Timeout;
        return (...args: Parameters<T>) => {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(() => func.apply(null, args), delay);
        };
    }

    // Throttle Function
    export function throttle<T extends (...args: any[]) => any>(
        func: T,
        delay: number
    ): (...args: Parameters<T>) => void {
        let lastExecution = 0;
        return (...args: Parameters<T>) => {
            const now = Date.now();
            if (now - lastExecution >= delay) {
                func.apply(null, args);
                lastExecution = now;
            }
        };
    }

    // Deep Clone
    export function deepClone<T>(obj: T): T {
        if (obj === null || typeof obj !== 'object') return obj;
        if (obj instanceof Date) return new Date(obj.getTime()) as any;
        if (obj instanceof Array) return obj.map(item => deepClone(item)) as any;
        if (typeof obj === 'object') {
            const cloned = {} as any;
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    cloned[key] = deepClone(obj[key]);
                }
            }
            return cloned;
        }
        return obj;
    }

    // Flatten Array
    export function flatten<T>(arr: (T | T[])[]): T[] {
        return arr.reduce<T[]>((acc, val) => 
            Array.isArray(val) ? acc.concat(flatten(val)) : acc.concat(val), []
        );
    }

    // Most Frequent Element
    export function mostFrequent<T>(arr: T[]): T | null {
        if (arr.length === 0) return null;
        
        const freqMap = new Map<T, number>();
        let maxCount = 0;
        let result = arr[0];
        
        for (const item of arr) {
            const count = (freqMap.get(item) || 0) + 1;
            freqMap.set(item, count);
            
            if (count > maxCount) {
                maxCount = count;
                result = item;
            }
        }
        
        return result;
    }

    // Statistical Functions
    export namespace Statistics {
        export function mean(numbers: number[]): number {
            return numbers.reduce((sum, num) => sum + num, 0) / numbers.length;
        }
        
        export function median(numbers: number[]): number {
            const sorted = [...numbers].sort((a, b) => a - b);
            const mid = Math.floor(sorted.length / 2);
            
            return sorted.length % 2 === 0
                ? (sorted[mid - 1] + sorted[mid]) / 2
                : sorted[mid];
        }
        
        export function mode(numbers: number[]): number[] {
            const freqMap = new Map<number, number>();
            let maxFreq = 0;
            
            for (const num of numbers) {
                const freq = (freqMap.get(num) || 0) + 1;
                freqMap.set(num, freq);
                maxFreq = Math.max(maxFreq, freq);
            }
            
            return Array.from(freqMap.entries())
                .filter(([, freq]) => freq === maxFreq)
                .map(([num]) => num);
        }
        
        export function standardDeviation(numbers: number[]): number {
            const avg = mean(numbers);
            const variance = numbers.reduce((sum, num) => sum + Math.pow(num - avg, 2), 0) / numbers.length;
            return Math.sqrt(variance);
        }
    }
}

// ================================
// CHAPTER 9: USAGE EXAMPLES
// ================================

export namespace DSAUsageExamples {
    export function runAllExamples(): void {
        console.log('=== DSA TypeScript Utilities Examples ===\n');

        // Array Examples
        console.log('1. Array Operations:');
        const nums = [2, 7, 11, 15];
        const twoSumResult = ArrayUtilities.twoSum(nums, 9);
        console.log(`Two Sum Result: [${twoSumResult.join(', ')}]`);
        
        const maxSubArray = [-2, 1, -3, 4, -1, 2, 1, -5, 4];
        console.log(`Max Subarray Sum: ${ArrayUtilities.maxSubArray(maxSubArray)}`);

        // String Examples
        console.log('\n2. String Operations:');
        console.log(`Is 'racecar' palindrome: ${StringUtilities.isPalindrome('racecar')}`);
        console.log(`Are 'listen' and 'silent' anagrams: ${StringUtilities.isAnagram('listen', 'silent')}`);

        // Linked List Examples
        console.log('\n3. Linked List Operations:');
        const list = LinkedListUtilities.fromArray([1, 2, 3, 4, 5]);
        const reversed = LinkedListUtilities.reverseList(list);
        console.log(`Reversed list: [${LinkedListUtilities.toArray(reversed).join(', ')}]`);

        // Tree Examples
        console.log('\n4. Tree Operations:');
        const root = new TreeNode(3);
        root.left = new TreeNode(9);
        root.right = new TreeNode(20);
        root.right.left = new TreeNode(15);
        root.right.right = new TreeNode(7);
        console.log(`Tree max depth: ${TreeUtilities.maxDepth(root)}`);

        // DP Examples
        console.log('\n5. Dynamic Programming:');
        console.log(`Fibonacci(10): ${DynamicProgrammingUtilities.fibonacci(10)}`);
        console.log(`Climb stairs (n=5): ${DynamicProgrammingUtilities.climbStairs(5)}`);

        // Advanced Examples
        console.log('\n6. Advanced Utilities:');
        const numbers = Array.from({length: 10}, (_, i) => i + 1);
        const slidingWindows = AdvancedUtilities.slidingWindow(numbers, 3).slice(0, 3);
        console.log('Sliding windows of size 3:');
        slidingWindows.forEach(window => {
            console.log(`[${window.join(', ')}]`);
        });

        const runningTotal = AdvancedUtilities.runningSum(numbers.slice(0, 5));
        console.log(`Running total: [${runningTotal.join(', ')}]`);

        // Statistical operations
        const data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        console.log(`Median: ${AdvancedUtilities.Statistics.median(data)}`);
        console.log(`Standard Deviation: ${AdvancedUtilities.Statistics.standardDeviation(data).toFixed(2)}`);
    }
}

// ================================
// TYPE DEFINITIONS FOR BETTER TS SUPPORT
// ================================

export type CompareFn<T> = (a: T, b: T) => number;
export type PredicateFn<T> = (item: T) => boolean;
export type MapFn<T, U> = (item: T, index?: number) => U;
export type ReduceFn<T, U> = (acc: U, current: T, index?: number) => U;

export interface GraphNode {
    value: number;
    neighbors: GraphNode[];
}

export interface WeightedEdge {
    from: number;
    to: number;
    weight: number;
}

// Default export for easy importing
export default {
    ArrayUtilities,
    StringUtilities,
    LinkedListUtilities,
    StackQueueUtilities,
    TreeUtilities,
    GraphUtilities,
    DynamicProgrammingUtilities,
    SortingSearchingUtilities,
    HashTableUtilities,
    HeapUtilities,
    AdvancedUtilities,
    DSAUsageExamples,
    ListNode,
    TreeNode
};

/* 
Usage Examples in your TypeScript projects:

// Import everything
import DSA from './DSA_TypeScript_Utilities';

// Or import specific namespaces
import { ArrayUtilities, StringUtilities, TreeUtilities } from './DSA_TypeScript_Utilities';

// Usage examples:
const result = ArrayUtilities.twoSum([2, 7, 11, 15], 9);
const isPalindrome = StringUtilities.isPalindrome('racecar');
const fibonacci = DynamicProgrammingUtilities.fibonacci(10);

// Run all examples
DSA.DSAUsageExamples.runAllExamples();

// Create data structures
const minHeap = new HeapUtilities.MinHeap();
minHeap.insert(5);
minHeap.insert(3);
console.log(minHeap.extractMin()); // 3

const lruCache = new HashTableUtilities.LRUCache(2);
lruCache.put(1, 1);
lruCache.put(2, 2);
console.log(lruCache.get(1)); // 1
*/