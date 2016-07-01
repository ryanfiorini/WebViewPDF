# Requiring this file will import Calabash and the Calabash predefined Steps.
if ENV['PLATFORM'] == 'ios'
  require 'calabash-cucumber/cucumber'
elsif ENV['PLATFORM'] == 'android'
  require 'calabash-android/cucumber'
end


class RunState
  @@first_run = true
  def self.run!
    @@first_run = false
  end
  def self.first_run?
    @@first_run
  end
end

# To use Calabash without the predefined Calabash Steps, uncomment these
# three lines and delete the require above.
# require 'calabash-cucumber/wait_helpers'
# require 'calabash-cucumber/operations'
# World(Calabash::Cucumber::Operations)
