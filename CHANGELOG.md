# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.3.0] - 2020-06-17
### Added
- `forceRefresh` param to PlayerInfo endpoint

## [2.2.0] - 2020-06-08
### Added
- Endpoint for retrieving MatchDataSets

## [2.1.1] - 2020-04-30
### Added
- MatchSelectionModel.InaccessibleMatches

### Changed
- Minor DateTime precision improvements and mock fixes related to MatchSelection

## [2.0.0] - 2020-04-30
### Added
- SubscriptionConfiguration from JSON
    - Loads `MatchAccessDurationInDays`, `DailyMatchesLimit` and `PositionFramesPerSecond` from a file on disk,
    - Applies these limits to the `MatchSelection` model factory.
- Environment Variables
    - `MOCK_STEAM_USER_OPERATOR`
    - `MOCK_SUBSCRIPTION_LOADER`

### Changed
- `MatchSelection` Controller now expects a `SubscriptionType` as a replacement for the `dailyLimit`.
- Default mock behavior of SteamUserOperator - Now requires an ENV Var

## [1.3.0] - 2020-04-22
### Added
- Change MatchSelection algorithm back to 24h limit resetting at midnight UTC
- Change DateTimeHandling to UTC

## [1.2.0] - 2020-04-22
### Added
- Add PlayerSummaryModel.Matches

## [1.1.0] - 2020-04-15
### Added
- Add MatchSelection.DailyLimitEnds

## [1.0.4] - 2020-04-15
### Changed
- DemoViewer now uses all available FPS regardless of requestedQuality, because the algorithm for reducing FPS was broken 

## [1.0.3] - 2020-04-08
### Added
- Parameter `ignoreMatchIds` in MatchesController

## [1.0.2] - 2020-04-08
### Changed
- Algorithm for MatchSelection to be timezone-agnostic and thereby less-allowing

## [1.0.0] - 2020-04-06
### Added
- Time field to Kills and Grenade samples
