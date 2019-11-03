﻿using ScmBackup.Hosters.Gitlab;
using ScmBackup.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScmBackup.Tests.Integration.Hosters
{
    public class GitlabApiTests : IHosterApiTests
    {
        internal override string HosterUser { get { return "scm-backup-testuser"; } }

        internal override string HosterOrganization { get { return "scm-backup-testgroup"; } }

        internal override string HosterRepo { get { return "scm-backup-test"; } }

        internal override string HosterCommit { get { return "d7c9ad8185b7707dbcc907e41154e3e5e5b2a540"; } }

        internal override string HosterWikiCommit { get { return "5893873f9da26fc59bbeaafde5fad5800907e56f"; } }

        internal override string HosterPaginationUser => throw new NotImplementedException();

        internal override string HosterPrivateRepo { get { return TestHelper.EnvVar(this.EnvVarPrefix, "RepoPrivate"); } }

        internal override string EnvVarPrefix { get { return "Gitlab"; } }

        internal override string ConfigHoster { get { return "gitlab"; } }

        internal override int Pagination_MinNumberOfRepos => throw new NotImplementedException();

        internal override bool SkipUnauthenticatedTests { get { return false; } }

        public GitlabApiTests()
        {
            this.sut = new GitlabApi(new HttpRequest());
        }
    }
}