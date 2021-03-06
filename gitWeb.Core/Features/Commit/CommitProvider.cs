﻿using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using gitWeb.Core.GraphBuilder;

namespace gitWeb.Core.Features.Commit
{
    public class CommitProvider : ICommitProvider
    {
        private readonly IRepository _repository;

        public CommitProvider(IRepository repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            _repository = repository;
        }

        public void Commit(string message, Signature author)
        {
            if (author == null)
            {
                author = new Signature("norek", "norekzal@gmail.com", DateTime.Now);
            }

            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException();
            if (author == null) throw new ArgumentNullException();

            var committer = author;

            _repository.Commit(message, author, committer);
        }

        public IEnumerable<Commit> GetAllFromHead()
        {
            var commits = _repository.Commits
                                //.Take(50)
                                .Select(d =>
                                            new Commit()
                                            {
                                                Date = d.Committer.When.DateTime,
                                                Name = d.Message,
                                                Parents = d.Parents.Select(p => p.Sha).ToList(),
                                                ParentsIds = d.Parents.Select(p => p.Id).ToList(),
                                                Sha = d.Sha,
                                                Message = d.MessageShort
                                            }).ToList();

            foreach (var branch in _repository.Branches)
            {
                var tipSha = branch.Tip.Sha;

                var reachableCommitFromBranchTip = commits.Where(d => d.Sha == tipSha);

                foreach (var item in reachableCommitFromBranchTip)
                {
                    item.AddReachableBranch(branch.FriendlyName);
                }
            }


            return commits;

        }

        public IEnumerable<Commit> GetAllFromTip(string branchName)
        {
            var commits = _repository.Branches[branchName].Commits
                           .Select(d =>
                                       new Commit()
                                       {
                                           Date = d.Committer.When.DateTime,
                                           Name = d.Message,
                                           Parents = d.Parents.Select(p => p.Sha).ToList(),
                                           Sha = d.Sha,
                                           Message = d.MessageShort
                                       }).Take(30).ToList();

            foreach (var branch in _repository.Branches)
            {
                var tipSha = branch.Tip.Sha;

                var reachableCommitFromBranchTip = commits.Where(d => d.Sha == tipSha);

                foreach (var item in reachableCommitFromBranchTip)
                {
                    item.AddReachableBranch(branch.FriendlyName);
                }
            }


            GraphBuilder.GraphBuilder builder = new GraphBuilder.GraphBuilder();
            builder.Build(commits.ToArray());

            return commits;
        }

        public CommitDetail GetCommitDetails(string sha)
        {
            if (string.IsNullOrEmpty(sha)) throw new ArgumentNullException();

            var commit = _repository.Lookup<LibGit2Sharp.Commit>(sha);

            if (commit == null) throw new CommitNotFoundException(sha);

            var commitParent = commit.Parents.Last();

            TreeChanges treeChanges = _repository.Diff.Compare<TreeChanges>(commitParent.Tree, commit.Tree);

            CommitDetail commitDetail = new CommitDetail(sha, commit.Message, commit.Author.Name, commit.Author.When.Date);
            commitDetail.Files.AddRange(treeChanges.Select(s => new CommitFile((int)s.Status, s.Path, Path.GetFileName(s.Path))));
            //commitDetail.Files.AddRange(treeChanges.Modified.Select(s => new CommitFile((int)s.Status, s.Path, Path.GetFileName(s.Path))));
            //commitDetail.Files.AddRange(treeChanges.Deleted.Select(s => new CommitFile((int)s.Status, s.Path, Path.GetFileName(s.Path))));

            return commitDetail;
        }
    }
}