delete from dbo."ReportedCompilerMessages" where "Date" < (now() - '3 months'::interval);
