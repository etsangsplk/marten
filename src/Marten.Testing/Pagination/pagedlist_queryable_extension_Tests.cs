﻿using System;
using System.Linq;
using Xunit;
using Shouldly;
using Marten.Services;
using Marten.Pagination;


namespace Marten.Testing.Pagination
{
    public class PaginationTestDocument
    {
        public string Id { get; set; }
    }

    public class pagedlist_queryable_extension_Tests : DocumentSessionFixture<NulloIdentityMap>
    {
        private void BuildUpTargetData()
        {
            var targets = Target.GenerateRandomData(100).ToArray();

            theSession.Store(targets);
            theSession.SaveChanges();
        }

        private void BuildUpDocumentWithZeroRecords()
        {
            var doc = new PaginationTestDocument();
            doc.Id = "test";

            theSession.Store(doc);
            theSession.SaveChanges();

            theSession.Delete<PaginationTestDocument>(doc);
            theSession.SaveChanges();
        }

        public pagedlist_queryable_extension_Tests()
        {
            BuildUpTargetData();
        }

        [Fact]
        public void can_return_paged_result()
        {
            var pageNumber = 2;
            var pageSize = 10;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);

            pagedList.Count.ShouldBe(pageSize);
        }

        [Fact]
        public void invalid_pagenumber_should_throw_exception()
        {
            // invalid page number
            var pageNumber = 0;

            var pageSize = 10;

            var ex =
                Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                    () => theSession.Query<Target>().AsPagedList(pageNumber, pageSize));
            ex.Message.ShouldContain("pageNumber = 0. PageNumber cannot be below 1.");
        }

        [Fact]
        public void invalid_pagesize_should_throw_exception()
        {
            var pageNumber = 1;

            // invalid page size
            var pageSize = 0;

            var ex =
                Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                    () => theSession.Query<Target>().AsPagedList(pageNumber, pageSize));
            ex.Message.ShouldContain($"pageSize = 0. PageSize cannot be below 1.");
        }

        [Fact]
        public void pagesize_outside_page_range_should_throw_exception()
        {
            // page number ouside the page range, page range is between 1 and 10 for the sample 
            var pageNumber = 11;

            var pageSize = 10;

            var ex =
                Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                    () => theSession.Query<Target>().AsPagedList(pageNumber, pageSize));
            ex.Message.ShouldContain($"pageNumber = 11. PageNumber is the outside the valid range.");
        }

        [Fact]
        public void check_computed_pagecount()
        {
            // page number ouside the page range, page range is between 1 and 10 for the sample 
            var pageNumber = 1;

            var pageSize = 10;

            var expectedPageCount = theSession.Query<Target>().Count()/pageSize;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.PageCount.ShouldBe(expectedPageCount);
        }

        [Fact]
        public void check_total_items_count()
        {
            var pageNumber = 1;

            var pageSize = 10;

            var expectedTotalItemsCount = theSession.Query<Target>().Count();

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.TotalItemCount.ShouldBe(expectedTotalItemsCount);
        }

        [Fact]
        public void check_has_previous_page()
        {
            var pageNumber = 2;

            var pageSize = 10;

            var expectedHasPreviousPage = true;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.HasPreviousPage.ShouldBe(expectedHasPreviousPage);
        }

        [Fact]
        public void check_has_no_previous_page()
        {
            var pageNumber = 1;

            var pageSize = 10;

            var expectedHasPreviousPage = false;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.HasPreviousPage.ShouldBe(expectedHasPreviousPage);
        }

        [Fact]
        public void check_has_next_page()
        { 
            var pageNumber = 1;

            var pageSize = 10;

            var expectedHasNextPage = true;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.HasNextPage.ShouldBe(expectedHasNextPage);
        }

        [Fact]
        public void check_has_no_next_page()
        {
            var pageNumber = 10;

            var pageSize = 10;

            var expectedHasNextPage = false;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.HasNextPage.ShouldBe(expectedHasNextPage);
        }

        [Fact]
        public void check_is_first_page()
        {
            var pageNumber = 1;

            var pageSize = 10;

            var expectedIsFirstPage = true;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.IsFirstPage.ShouldBe(expectedIsFirstPage);
        }

        [Fact]
        public void check_is_not_first_page()
        {
            var pageNumber = 2;

            var pageSize = 10;

            var expectedIsFirstPage = false;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.IsFirstPage.ShouldBe(expectedIsFirstPage);
        }

        [Fact]
        public void check_is_last_page()
        {
            var pageNumber = 10;

            var pageSize = 10;

            var expectedIsLastPage = true;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.IsLastPage.ShouldBe(expectedIsLastPage);
        }

        [Fact]
        public void check_is_not_last_page()
        {
            var pageNumber = 1;

            var pageSize = 10;

            var expectedIsLastPage = false;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.IsLastPage.ShouldBe(expectedIsLastPage);
        }

        [Fact]
        public void check_first_item_on_page()
        {
            var pageNumber = 2;

            var pageSize = 10;

            var expectedFirstItemOnPage = 11;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.FirstItemOnPage.ShouldBe(expectedFirstItemOnPage);
        }

        [Fact]
        public void check_last_item_on_page()
        {
            var pageNumber = 2;

            var pageSize = 10;

            var expectedLastItemOnPage = 20;

            var pagedList = theSession.Query<Target>().AsPagedList(pageNumber, pageSize);
            pagedList.LastItemOnPage.ShouldBe(expectedLastItemOnPage);
        }

        [Fact]
        public void zero_records_document_should_return_pagedlist_gracefully()
        {
            BuildUpDocumentWithZeroRecords();

            var pageNumber = 1;

            var pageSize = 10;

            var pagedList = theSession.Query<PaginationTestDocument>().AsPagedList(pageNumber, pageSize);
            pagedList.TotalItemCount.ShouldBe(0);
            pagedList.PageCount.ShouldBe(0);
            pagedList.IsFirstPage.ShouldBe(false);
            pagedList.IsLastPage.ShouldBe(false);
            pagedList.HasPreviousPage.ShouldBe(false);
            pagedList.HasNextPage.ShouldBe(false);
            pagedList.FirstItemOnPage.ShouldBe(0);
            pagedList.LastItemOnPage.ShouldBe(0);
        }
    }
}
