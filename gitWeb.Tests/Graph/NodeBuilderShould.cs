﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gitWeb.Core.Features.Commit;
using gitWeb.Core.GraphBuilder;
using Xunit;

namespace gitWeb.Tests.Graph
{
    public class LinkBuilderTest
    {
        private readonly CommitCreator _commitCreator = new CommitCreator();
        private readonly LinkBuilder _linkBuilder = new LinkBuilder();

        [Fact]
        public void WhenCommitHasOneParent_BuilderShouldLinkedHimWithOneLine()
        {
            var parentCommit = _commitCreator.CreateNewCommit();
            var commit = _commitCreator.CreateNewCommit(parentCommit.Sha);

            var links = _linkBuilder.Build(new[] { commit, parentCommit }.ToDictionary(k => k.Sha, v => v));

            Assert.Equal(1, links.Count);
            Assert.Equal(new Link(commit, parentCommit), links.First());
        }

        [Fact]
        public void WhenCommitHasMoreThanOneParent_CreateLinksNumberOfLinksEqualsToNumberOfParents()
        {
            var parentCommit = _commitCreator.CreateNewCommit();
            var parentCommit1 = _commitCreator.CreateNewCommit();
            var parentCommit2 = _commitCreator.CreateNewCommit();
            var commit = _commitCreator.CreateNewCommit(parentCommit.Sha, parentCommit1.Sha, parentCommit2.Sha);

            var links = _linkBuilder.Build(new[] { commit, parentCommit, parentCommit1, parentCommit2 }.ToDictionary(k => k.Sha, v => v));

            Assert.Equal(3, links.Count);
            Assert.Equal(new Link(commit, parentCommit), links[0]);
            Assert.Equal(new Link(commit, parentCommit1), links[1]);
            Assert.Equal(new Link(commit, parentCommit2), links[2]);
        }

        [Fact]
        public void WhenTwoCommitsHasSameParent_BuilderShouldCreateOneLinkPerRelation()
        {
            var parentCommit = _commitCreator.CreateNewCommit();
            var commit1 = _commitCreator.CreateNewCommit(parentCommit.Sha);
            var commit2 = _commitCreator.CreateNewCommit(parentCommit.Sha);

            var links = _linkBuilder.Build(new[] { commit1, commit2, parentCommit}.ToDictionary(k => k.Sha,v => v));

            Assert.Equal(2, links.Count);
            Assert.Equal(new Link(commit1, parentCommit), links[0]);
            Assert.Equal(new Link(commit2, parentCommit), links[1]);
        }
        
    }
}
