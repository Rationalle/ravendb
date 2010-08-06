require "rss/1.0"

module RSS

  SY_PREFIX = 'sy'
  SY_URI = "http://purl.org/rss/1.0/modules/syndication/"

  RDF.install_ns(SY_PREFIX, SY_URI)

  module SyndicationModel
    
    extend BaseModel
    
    ELEMENTS = []
    
    def self.append_features(klass)
      super

      klass.install_must_call_validator(SY_PREFIX, SY_URI)
      klass.module_eval do
        [
          ["updatePeriod"],
          ["updateFrequency", :positive_integer]
        ].each do |name, type|
          install_text_element(name, SY_URI, "?",
                               "#{SY_PREFIX}_#{name}", type,
                               "#{SY_PREFIX}:#{name}")
        end

        %w(updateBase).each do |name|
          install_date_element(name, SY_URI, "?",
                               "#{SY_PREFIX}_#{name}", 'w3cdtf', name)
        end

        alias_method(:_sy_updatePeriod=, :sy_updatePeriod=)
        def sy_updatePeriod=(new_value)
          new_value = new_value.strip
          validate_sy_updatePeriod(new_value) if @do_validate
          self._sy_updatePeriod = new_value
        end
      end
    end

    private
    SY_UPDATEPERIOD_AVAILABLE_VALUES = %w(hourly daily weekly monthly yearly)
    def validate_sy_updatePeriod(value)
      unless SY_UPDATEPERIOD_AVAILABLE_VALUES.include?(value)
        raise NotAvailableValueError.new("updatePeriod", value)
      end
    end
  end

  class RDF
    class Channel; include SyndicationModel; end
  end

  prefix_size = SY_PREFIX.size + 1
  SyndicationModel::ELEMENTS.uniq!
  SyndicationModel::ELEMENTS.each do |full_name|
    name = full_name[prefix_size..-1]
    BaseListener.install_get_text_element(SY_URI, name, "#{full_name}=")
  end

end