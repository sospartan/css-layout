/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using NUnit.Framework;
using System;

/**
 * Tests for {@link CSSNode}.
 */
namespace Facebook.CSSLayout
{
    [TestFixture]
    public class CSSNodeTest
    {
        [Test]
        public void TestAddChildGetParent()
        {
            CSSNode parent = new CSSNode();
            CSSNode child = new CSSNode();
    
            Assert.IsNull(child.Parent);
            Assert.AreEqual(0, parent.Count);
    
            parent.Insert(0, child);
    
            Assert.AreEqual(1, parent.Count);
            Assert.AreEqual(child, parent[0]);
            Assert.AreEqual(parent, child.Parent);
    
            parent.RemoveAt(0);
    
            Assert.IsNull(child.Parent);
            Assert.AreEqual(0, parent.Count);
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestCannotAddChildToMultipleParents()
        {
            CSSNode parent1 = new CSSNode();
            CSSNode parent2 = new CSSNode();
            CSSNode child = new CSSNode();

            parent1.Insert(0, child);
            parent2.Insert(0, child);
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestAlreadyInitialize()
        {
            CSSNode node = new CSSNode();
            node.Reinitialize();
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestNullNativePointer()
        {
            CSSNode node = new CSSNode();
            node.Free();
            node.CalculateLayout();
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestDoubleFree()
        {
            CSSNode node = new CSSNode();
            node.Free();
            node.Free();
        }

        [Test]
        public void TestReinitialize()
        {
            CSSNode node = new CSSNode();
            node.Free();
            node.Reinitialize();
        }

        [Test]
        [ExpectedException("System.ObjectDisposedException")]
        public void TestDisposed()
        {
            CSSNode node = new CSSNode();
            node.Dispose();
            node.CalculateLayout();
        }

        [Test]
        public void TestFree()
        {
            CSSNode node = new CSSNode();
            node.Free();
        }

        [Test]
        public void TestDispose()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            CSSNode node = new CSSNode();
            Assert.AreEqual(1, CSSNode.GetInstanceCount());
            node.Dispose();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
        }

        [Test]
        public void TestDisposeWithUsing()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            using (CSSNode node = new CSSNode())
            {
                Assert.AreEqual(1, CSSNode.GetInstanceCount());
            }
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
        }

        [Test]
        public void TestDestructor()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            TestDestructorForGC();
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
        }

        private void TestDestructorForGC()
        {
            CSSNode node = new CSSNode();
            Assert.IsNotNull(node);
            Assert.AreEqual(1, CSSNode.GetInstanceCount());
            node = null;
        }

        [Test]
        public void TestDestructorWithChildren()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            TestDestructorWithChildrenForGC1();
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
        }

        private void TestDestructorWithChildrenForGC1()
        {
            CSSNode node = new CSSNode();
            Assert.AreEqual(1, CSSNode.GetInstanceCount());

            TestDestructorWithChildrenForGC2(node, 1);
            ForceGC();
            Assert.AreEqual(2, CSSNode.GetInstanceCount());

            TestDestructorWithChildrenForGC2(node, 2);
            ForceGC();
            Assert.AreEqual(3, CSSNode.GetInstanceCount());

            node = null;
        }

        private void TestDestructorWithChildrenForGC2(CSSNode parent, int count)
        {
            CSSNode child = new CSSNode();
            Assert.AreEqual(count + 1, CSSNode.GetInstanceCount());

            parent.Insert(0, child);
            child = null;
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestFreeParent()
        {
            CSSNode parent = new CSSNode();
            CSSNode child = new CSSNode();
            parent.Insert(0, child);
            parent.Free();
        }

        [Test]
        [ExpectedException("System.InvalidOperationException")]
        public void TestFreeChild()
        {
            CSSNode parent = new CSSNode();
            CSSNode child = new CSSNode();
            parent.Insert(0, child);
            child.Free();
        }

        [Test]
        public void TestDisposeParent()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            CSSNode parent = new CSSNode();
            Assert.AreEqual(1, CSSNode.GetInstanceCount());
            TestDisposeParentForGC(parent);
            ForceGC();
            Assert.AreEqual(2, CSSNode.GetInstanceCount());
            parent.Dispose();
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
        }

        private void TestDisposeParentForGC(CSSNode parent)
        {
            CSSNode child = new CSSNode();
            Assert.AreEqual(2, CSSNode.GetInstanceCount());
            parent.Insert(0, child);
            child = null;
        }

        [Test]
        public void TestDisposeChild()
        {
            ForceGC();
            Assert.AreEqual(0, CSSNode.GetInstanceCount());
            CSSNode parent = new CSSNode();
            CSSNode child0 = new CSSNode();
            CSSNode child1 = new CSSNode();
            Assert.AreEqual(3, CSSNode.GetInstanceCount());
            Assert.AreEqual(0, parent.Count);
            parent.Insert(0, child1);
            Assert.AreEqual(0, parent.IndexOf(child1));
            parent.Insert(0, child0);
            Assert.AreEqual(0, parent.IndexOf(child0));
            Assert.AreEqual(1, parent.IndexOf(child1));
            child0.Dispose();
            Assert.AreEqual(2, CSSNode.GetInstanceCount());
            Assert.AreEqual(1, parent.Count);
            Assert.AreEqual(0, parent.IndexOf(child1));
        }

        [Test]
        public void TestMeasureFunc()
        {
            CSSNode node = new CSSNode();
            node.SetMeasureFunction((_, width, widthMode, height, heightMode, measureResult) => {
                measureResult.Width = 100;
                measureResult.Height = 150;
            });
            node.CalculateLayout();
            Assert.AreEqual(100, (int)node.LayoutWidth);
            Assert.AreEqual(150, (int)node.LayoutHeight);
        }

        private void ForceGC()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
        }
    }
}
